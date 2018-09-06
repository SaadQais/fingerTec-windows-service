using AxBioBridgeSDKv3;
using FingerTecWindowsService.Models;
using FingerTecWindowsService.Models.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FingerTecWindowsService.Helper
{
    public class Operations
    {
        AxBioBridgeSDKv3lib sdk;
        LogWriter lw = new LogWriter();
        MyDbContext db = new MyDbContext();

        public Operations()
        {
            this.sdk = new AxBioBridgeSDKv3lib();
        }

        public void Run()
        {
            try
            {
                foreach (var device in db.Devices.ToList())
                {
                    sdk = new AxBioBridgeSDKv3lib();
                    sdk.CreateControl();

                    int port = Convert.ToInt32(device.Port);
                    int key = Convert.ToInt32(device.Key);

                    if (sdk.Connect_TCPIP("", device.DeviceId, device.IP, port, key) == 0)
                    {
                        lw.LogWrite("Device connected");

                        int year = DateTime.Now.Year;
                        int month = DateTime.Now.Month;

                        List<Log> deviceLog = GetDeviceLogs(device, year, month);
                        List<Log> logsFromDB = GetLogsFromDB(device, year, month);

                        foreach (var log in deviceLog)
                        {
                            if (!logsFromDB.Any(l => l.EmployeeId == log.EmployeeId &&
                                     l.CheckTime == log.CheckTime
                                     && log.CheckDate.Value.Year == l.CheckDate.Value.Year
                                     && log.CheckDate.Value.Month == l.CheckDate.Value.Month
                                     && log.CheckDate.Value.Day == l.CheckDate.Value.Day))
                            {
                                lw.LogWrite(log.EmployeeId + " " + log.CheckDate);

                                FPInfo info = new FPInfo
                                {
                                    EmployeeId = log.EmployeeId,
                                    CheckDate = log.CheckDate.Value,
                                    CheckTime = log.CheckTime.Value,
                                    DeviceId = log.DeviceId
                                };

                                if (info.EmployeeId != null)
                                {
                                    new LogsHelper().InsertNewLog(info);
                                }
                            }
                        }
                    }
                    else
                    {
                        lw.LogWrite("Device not connected");
                    }
                }
            }
            catch(System.Exception e)
            {
                lw.LogWrite(e.Message);
            }
        }

        private Employee GenerateNewUser(string enrollNo)
        {
            string Name = "";
            string pwd = "0";
            int level = 0;
            long CardNo;
            bool status = false;
            sdk.cardNo = 0;

            if ((sdk.SSR_GetUserInfo(enrollNo, ref Name, ref pwd, ref level, ref status) == 0))
            {
                try
                {
                    CardNo = sdk.cardNo;

                    new LogsHelper().GenerateDefaultEmployee(Name, Convert.ToInt32(enrollNo));
                }
                catch (System.Exception e)
                {
                    lw.LogWrite(e.Message);
                }
            }
            else
            {
                lw.LogWrite("No, record, found, !");
            }

            return null;
        }

        private List<Log> GetLogsFromDB(Device device, int year, int month)
        {
            DateTime LastDateOfMonth = new DateTime(year, month, 1).Subtract(new TimeSpan(1, 0, 0, 0));

            var logsFromDate = db.Logs.Where(l => (l.CheckDate.Value.Year == year && l.CheckDate.Value.Month
                == month) || (LastDateOfMonth.Date == DbFunctions.TruncateTime(l.CheckDate.Value))).ToList();

            return logsFromDate;
        }

        private List<Log> GetDeviceLogs(Device device, int year, int month)
        {
            string enrollNo = "";
            int yr = 0;
            int mth = 0;
            int day_Renamed = 0;
            int hr = 0;
            int min = 0;
            int sec = 0;
            int ver = 0;
            int io = 0;
            int work = 0;
            int iSize = 0;
            //int ioo = 0;

            List<string> Temp = new List<string>();

            List<Log> logs = new List<Log>();

            sdk.CreateControl();

            if (sdk.Connect_TCPIP("", device.DeviceId, device.IP, Convert.ToInt16(device.Port), Convert.ToInt16(device.Key)) == 0)
            {
                if (sdk.ReadGeneralLog(ref iSize) == 0)
                {
                    while (sdk.SSR_GetGeneralLog(ref enrollNo, ref yr, ref mth, ref day_Renamed, ref hr, ref min,
                        ref sec, ref ver, ref io, ref work) == 0)
                    {
                        DateTime LogDate = new DateTime(yr, mth, day_Renamed);
                        DateTime FirstDateOfMonth = new DateTime(year, month, 1);

                        if ((LogDate.Year == year && LogDate.Month == month) ||
                            (FirstDateOfMonth.Subtract(LogDate).TotalDays == 1))
                        {
                            var FprintUser = new LogsHelper().GetEmployeeInfo(enrollNo);

                            if (FprintUser == null)
                                FprintUser = GenerateNewUser(enrollNo);

                            int? employeeId = new LogsHelper().GetEmployeeId(enrollNo);

                            if (employeeId != null)
                            {
                                logs.Add(new Log
                                {
                                    EmployeeId = employeeId.GetValueOrDefault(),
                                    CheckDate = new DateTime(yr, mth, day_Renamed),
                                    CheckTime = new TimeSpan(hr, min, sec),
                                    DeviceId = (device.Id != null) ? device.Id : 0
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                lw.LogWrite(DateTime.Now + " " + "cannot get logs from device " + device.DeviceName);
            }

            return logs;
        }

    }
}
