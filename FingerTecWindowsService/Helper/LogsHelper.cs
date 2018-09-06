using FingerTecWindowsService.Models;
using FingerTecWindowsService.Models.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FingerTecWindowsService.Helper
{

    public class LogsHelper
    {
        MyDbContext db = new MyDbContext();
        LogWriter lw = new LogWriter();

        public void InsertNewLog(FPInfo info)
        {
            try
            {
                var user = db.Employees.Find(info.EmployeeId);

                db.Logs.Add(new Log
                {
                    CheckDate = info.CheckDate,
                    EmployeeId = info.EmployeeId.GetValueOrDefault(),
                    CheckTime = info.CheckTime,
                    CheckType = 3,
                    DeviceId = info.DeviceId
                });

                db.SaveChanges();

                //complete adding log process

                GenearateParsedLog(info.EmployeeId.GetValueOrDefault(), info.CheckDate, info.CheckTime);
            }
            catch (System.Exception e)
            {
                lw.LogWrite(e.Message);
            }
        }

        private void GenearateParsedLog(int employeeId, DateTime checkDate, TimeSpan checkTime)
        {
            var userParsedLogs = db.ParsedLogs.Where(l => l.EmployeeId == employeeId &&
                DbFunctions.TruncateTime(l.CheckDate) == DbFunctions.TruncateTime(checkDate))
                .ToList();

            if (userParsedLogs.Count == 0)
            {
                db.ParsedLogs.Add(new ParsedLog()
                {
                    CheckDate = checkDate,
                    CheckIn = checkTime,
                    EmployeeId = employeeId
                });

                db.SaveChanges();
            }

            else
            {
                CheckNextLog(checkTime, userParsedLogs);
            }
        }

        private void CheckNextLog(TimeSpan checkTime, List<ParsedLog> userParsedLogs)
        {
            if (userParsedLogs.First() != null)
            {
                var userParse = userParsedLogs.First();

                if (userParse.CheckIn == null)
                {
                    //insert checkin 
                    userParsedLogs.First().CheckIn = checkTime;
                }

                else if (userParse.CheckOut != null)
                {
                    int checkStatus = CheckLogTime(checkTime, userParse);

                    if (checkStatus == 0)
                    {
                        userParsedLogs.First().CheckIn = checkTime;
                    }

                    if (checkStatus == 1)
                    {
                        userParsedLogs.First().CheckOut = checkTime;
                    }
                }

                else
                {
                    //check to insert checkout 
                    if (!CheckTimeDifference(checkTime, userParse.CheckIn.Value))
                    {
                        if (checkTime > userParse.CheckIn.Value)
                        {
                            userParsedLogs.First().CheckOut = checkTime;
                        }
                        else
                        {
                            userParsedLogs.First().CheckOut = userParsedLogs.First().CheckIn;
                            userParsedLogs.First().CheckIn = checkTime;
                        }
                    }
                    else
                    {
                        if (checkTime < userParse.CheckIn.Value)
                        {
                            userParsedLogs.First().CheckIn = checkTime;
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        //return 0 to replace check in, 1 to replace check out and 2 to neglect the value
        private int CheckLogTime(TimeSpan? checkTime, ParsedLog userParsedLogs)
        {
            if (checkTime <= userParsedLogs.CheckIn)
            {
                if (CheckTimeDifference(userParsedLogs.CheckIn.Value, checkTime.Value))
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }

            if (checkTime >= userParsedLogs.CheckIn && checkTime <= userParsedLogs.CheckOut)
            {
                if (CheckTimeDifference(checkTime.Value, userParsedLogs.CheckIn.Value) ||
                        CheckTimeDifference(checkTime.Value, userParsedLogs.CheckIn.Value))
                {
                    return 2;
                }
            }

            if (checkTime >= userParsedLogs.CheckOut)
            {
                if (CheckTimeDifference(userParsedLogs.CheckOut.Value, checkTime.Value))
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }

            return 2;
        }

        //method to compare two timespans and check if deffrence less than 5 minutes
        private bool CheckTimeDifference(TimeSpan time1, TimeSpan time2)
        {
            double minutes = Math.Abs((time1 - time2).TotalMinutes);

            if (minutes <= TimeSpan.FromMinutes(5).TotalMinutes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int? GetEmployeeId(string enrollNo)
        {
            if (int.TryParse(enrollNo, out int enroll))
            {
                var employee = db.Employees.Where(u => u.EnrollNo == enroll).FirstOrDefault();

                if (employee != null)
                    return employee.Id;
            }

            return null;
        }

        public void GenerateDefaultEmployee(string name, int enroll)
        {
            if (!string.IsNullOrEmpty(name))
            {
                db.Employees.Add(new Employee
                {
                    EnrollNo = enroll,
                    Name = name
                });
            }

            db.SaveChanges();
        }

        public Employee GetEmployeeInfo(string enrollNo)
        {
            int enNo = Convert.ToInt32(enrollNo);
            var EmployeeInfo = db.Employees.FirstOrDefault(e => e.EnrollNo == enNo);

            if (EmployeeInfo != null)
            {
                return EmployeeInfo;
            }

            return null;
        }
    }
}
