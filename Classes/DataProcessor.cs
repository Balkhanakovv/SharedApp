using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace SharedApp.Classes
{
    class DataProcessor
    {
        public UsersTbl user { get; private set; }

        private UsersTbl SenderUser;

        public FileStruct fileStruct { get; private set; }

        public List<FilesTbl> allFiles { get; private set; }

        public string BanReason { get; private set; }

        public DataProcessor() { }

        public List <CountriesTbl> GetCountriesList()
        {
            using (sharefiledbContext db = new sharefiledbContext())
            {
                return db.CountriesTbls.ToList();
            }
        }

        public List <BansTbl> GetBanList()
        {
            using (sharefiledbContext db = new sharefiledbContext())
            {
                return db.BansTbls.ToList();
            }
        }

        public List <AccessRightsTbl> GetAccessRights()
        {
            using (sharefiledbContext db = new sharefiledbContext())
            {
                return db.AccessRightsTbls.ToList();
            }
        }

        public List <FileTypeTbl> GetFileTypes()
        {
            using (sharefiledbContext db = new sharefiledbContext())
            {
                return db.FileTypeTbls.ToList();
            }
        }

        public List <TrafficPlanTbl> GetTrafficPlans()
        {
            using (sharefiledbContext db = new sharefiledbContext())
            {
                return db.TrafficPlanTbls.ToList();
            }
        }

        public List <UserLevelsTbl> GetUserLevels()
        {
            using (sharefiledbContext db = new sharefiledbContext())
            {
                return db.UserLevelsTbls.ToList();
            }
        }

        public byte LoginFunction (string Login, string Password)
        {
            //
            //Данная функция возвращает код результата операции:
            //      0 - Операция прошла успешно (логин и пароль совпали с данными в БД);
            //      1 - Ошибка входа (Данный пользователь не найден в БД);
            //      2 - Ошибка входа (Неверный пароль);
            //      3 - Ошибка подключения к БД (если сервер БД не отвечает)
            //

            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {
                    user = db.UsersTbls.FirstOrDefault(p => p.UserId == Login);

                    if (user == null)
                    {
                        return 0x01;
                    }

                    if (user.Passwd != Encrypt(Password))
                    {
                        return 0x02;
                    }
                    else
                    {
                        CheckFiles();

                        return 0x00;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0x03;
            }
        }

        public byte RegistrationFunction(string Login, string Password, string countryStr = null)
        {
            //
            //Данная функция возвращает код результата операции:
            //      0   - Операция прошла успешно (Пользователь добавлен в БД);
            //      1   - Ошибка операции (Пользователь с таким именем уже существует)
            //



            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {

                    if (countryStr != null)
                    {
                        CountriesTbl country = db.CountriesTbls.FirstOrDefault(p => p.CountryName == countryStr);
                        UserLevelsTbl level = db.UserLevelsTbls.FirstOrDefault(p => p.LevelName == "user");
                        TrafficPlanTbl plan = db.TrafficPlanTbls.FirstOrDefault(p => p.TrafficPlan == 100);

                        user = new UsersTbl 
                        { 
                            UserId = Login, 
                            Passwd = Encrypt(Password), 
                            MemorySize = 0, 
                            CountryId = country.CountryId, 
                            LevelId = level.LevelId, 
                            PlanId = plan.PlanId 
                        };
                        db.UsersTbls.Add(user);
                        db.SaveChanges();

                        return 0x00;
                    }
                    else
                    {
                        UserLevelsTbl level = db.UserLevelsTbls.FirstOrDefault(p => p.LevelName == "user");
                        TrafficPlanTbl plan = db.TrafficPlanTbls.FirstOrDefault(p => p.TrafficPlan == 100);

                        user = new UsersTbl { UserId = Login, Passwd = Encrypt(Password), MemorySize = 0, LevelId = level.LevelId, PlanId = plan.PlanId };
                        db.UsersTbls.Add(user);
                        db.SaveChanges();

                        return 0x00;
                    }
                }
            }
            catch
            {
                return 0x01;
            }
        }

        public void UploadFile(string FilePath, int FileType, int AccessRight)
        {
            //Чтение файла 
            using (FileStream fs = File.OpenRead(FilePath))
            {
                byte[] data = new byte[fs.Length];

                FileInfo fileInfo = new FileInfo(FilePath);

                fileStruct = new FileStruct();
                fileStruct.FileName = fileInfo.Name;
                fileStruct.FileSize = (int)fileInfo.Length;
                fileStruct.FileData = data;

                fs.Read(data, 0, data.Length);
            }

            FilesTbl file;

            //добавление его в базу данных
            using (sharefiledbContext db = new sharefiledbContext())
            {
                

                if (FileType != -1)
                {
                    FileTypeTbl fileType = db.FileTypeTbls.FirstOrDefault(p => p.TypeId == FileType + 1);

                    file = new FilesTbl
                    {
                        FileNam = fileStruct.FileName,
                        FileBin = fileStruct.FileData,
                        FileSize = fileStruct.FileSize,
                        CreateDate = DateTime.Now.Date,
                        TypeId = fileType.TypeId
                    };
                }
                else
                {
                    file = new FilesTbl
                    {
                        FileNam = fileStruct.FileName,
                        FileBin = fileStruct.FileData,
                        FileSize = fileStruct.FileSize,
                        CreateDate = DateTime.Now.Date
                    };
                }

                if (db.TrafficPlanTbls.FirstOrDefault(p => p.PlanId == user.PlanId).TrafficPlan * (1024 * 1024) > user.MemorySize + file.FileSize)
                {
                    user = db.UsersTbls.FirstOrDefault(p => p.UserId == user.UserId);
                    user.MemorySize += Math.Round(((double)file.FileSize / 1024 / 1024), 3);
                    db.UsersTbls.Update(user);
                    db.SaveChanges();
                }
                else
                    return;

                db.FilesTbls.Add(file);
                db.SaveChanges();
            }

            //Добавление документа, который ссылается на файл
            using (sharefiledbContext db = new sharefiledbContext())
            {
                DocumentsTbl document;

                if (AccessRight != -1)
                {
                    AccessRightsTbl accessRight = db.AccessRightsTbls.FirstOrDefault(p => p.RightId == AccessRight + 1);

                    document = new DocumentsTbl { UserId = user.UserId, FileId = file.FileId, RightId = accessRight.RightId };
                }
                else
                {
                    document = new DocumentsTbl { UserId = user.UserId, FileId = file.FileId, RightId = 2 };
                }

                db.DocumentsTbls.Add(document);
                db.SaveChanges();
            }

            CheckFiles();
        }

        public void DeleteFile(FilesTbl SelectedFile)
        {
            using (sharefiledbContext db = new sharefiledbContext())
            {
                //db.FilesTbls.FromSqlRaw($"DELETE Documents_Tbl WHERE DocumentId == {SelectedFile.FileId}; DELETE Files_tbl WHERE FileId = {SelectedFile.FileId}");
                TransactionsTbl transact = db.TransactionsTbls.FirstOrDefault(p => p.DocumentId == SelectedFile.FileId);
                if (transact != null)
                    db.TransactionsTbls.Remove(transact);

                db.DocumentsTbls.Remove(db.DocumentsTbls.First(p => p.DocumentId == SelectedFile.FileId));
                db.FilesTbls.Remove(db.FilesTbls.First(p => p.FileId == SelectedFile.FileId));

                db.SaveChanges();

                user = db.UsersTbls.FirstOrDefault(p => p.UserId == this.user.UserId);
                user.MemorySize -= (double)SelectedFile.FileSize / 1024 / 1024;

                if (user.MemorySize < 0)
                    user.MemorySize = 0;

                db.SaveChanges();
            }

            CheckFiles();
        }

        public int SearchUser(string UserName)
        {
            //
            //
            //Метод поиска пользователей и 
            //  получения доступных файлов
            //

            using (sharefiledbContext db = new sharefiledbContext())
            {
                SenderUser = db.UsersTbls.FirstOrDefault(p => p.UserId == UserName);

                if (SenderUser == null)
                    return 1;

                if (SenderUser == this.user)
                    return 1;

                CheckFiles(SenderUser);
                return 0;
            }
        }

        public void AddFiles(FilesTbl SelectedFile)
        {
            FilesTbl AddedFile;

            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {
                    if (db.TrafficPlanTbls.FirstOrDefault(p => p.PlanId == user.PlanId).TrafficPlan * (1024 * 1024) > user.MemorySize + SelectedFile.FileSize)
                    {
                        user = db.UsersTbls.FirstOrDefault(p => p.UserId == user.UserId);
                        user.MemorySize += Math.Round(((double)SelectedFile.FileSize / 1024 / 1024), 3);
                        db.UsersTbls.Update(user);
                        db.SaveChanges();
                    }
                    else
                        return;

                    AddedFile = new FilesTbl
                    {
                        FileNam = SelectedFile.FileNam,
                        FileBin = db.FilesTbls.FirstOrDefault(p => p.FileId == SelectedFile.FileId).FileBin,
                        FileSize = SelectedFile.FileSize,
                        CreateDate = SelectedFile.CreateDate,
                        TypeId = db.FilesTbls.FirstOrDefault(p => p.FileId == SelectedFile.FileId).TypeId
                    };

                    db.FilesTbls.Add(AddedFile);

                    db.SaveChanges();
                }

                using (sharefiledbContext db = new sharefiledbContext())
                {
                    //FilesTbl file = db.FilesTbls.FirstOrDefault(p => p.FileId == AddedFile.FileId);
                    //DocumentsTbl document;

                    DocumentsTbl document = new DocumentsTbl { UserId = user.UserId, FileId = AddedFile.FileId, RightId = 1 };
                    db.DocumentsTbls.Add(document);

                    db.SaveChanges();

                    TransactionsTbl transact = new TransactionsTbl { Sender = SenderUser.UserId, Receiver = user.UserId, DocumentId = document.DocumentId, TransactionTime = DateTime.Today};
                    db.TransactionsTbls.Add(transact);

                    db.SaveChanges();
                }

                AddedFile = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void CheckFiles()
        {
            //
            //
            //Метод на получение файлов активного пользователя
            //
            //

            if (user != null)
            {
                try
                {
                    using (sharefiledbContext db = new sharefiledbContext())
                    {
                        List<DocumentsTbl> allDocuments = db.DocumentsTbls.Where(p => p.UserId == user.UserId).ToList();
                        allFiles = new List<FilesTbl>();

                        foreach (DocumentsTbl doc in allDocuments)
                        {
                            int FileId = db.FilesTbls.Where(p => p.FileId == doc.FileId).Select(p => p.FileId).SingleOrDefault();
                            string FileName = db.FilesTbls.Where(p => p.FileId == doc.FileId).Select(p => p.FileNam).SingleOrDefault();
                            int FileSize = db.FilesTbls.Where(p => p.FileId == doc.FileId).Select(p => p.FileSize).SingleOrDefault();
                            DateTime CreateDate = db.FilesTbls.Where(p => p.FileId == doc.FileId).Select(p => p.CreateDate).SingleOrDefault();

                            FilesTbl file = new FilesTbl {FileId = FileId, FileNam = FileName, FileSize = FileSize, CreateDate = CreateDate };
                            allFiles.Add(file);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void CheckFiles(UsersTbl user)
        {
            //
            //
            //Метод на получение файлов конкретного пользователя
            //
            //

            if (user != null)
            {
                try
                {
                    using (sharefiledbContext db = new sharefiledbContext())
                    {
                        var canDownload = db.AccessRightsTbls.First(p => p.AccessRight == "Download");

                        List<DocumentsTbl> allDocuments = db.DocumentsTbls.Where(p => p.UserId == user.UserId).ToList();
                        allFiles = new List<FilesTbl>();

                        foreach (DocumentsTbl doc in allDocuments)
                        {
                            if (doc.RightId == canDownload.RightId)
                            {
                                int FileId = db.FilesTbls.Where(p => p.FileId == doc.FileId).Select(p => p.FileId).SingleOrDefault();
                                string FileName = db.FilesTbls.Where(p => p.FileId == doc.FileId).Select(p => p.FileNam).SingleOrDefault();
                                int FileSize = db.FilesTbls.Where(p => p.FileId == doc.FileId).Select(p => p.FileSize).SingleOrDefault();
                                DateTime CreateDate = db.FilesTbls.Where(p => p.FileId == doc.FileId).Select(p => p.CreateDate).SingleOrDefault();

                                FilesTbl file = new FilesTbl { FileId = FileId, FileNam = FileName, FileSize = FileSize, CreateDate = CreateDate };
                                allFiles.Add(file);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public FormatedUser GetUserInformation()
        {
            //
            // Получение информации о пользователе в читаемом виде.
            // 
            // Метод возвращает структуру, содержащую инормацию о пользователе
            //
            //

                try
                {
                    using (sharefiledbContext db = new sharefiledbContext())
                    {
                        string login = user.UserId;
                        string level = db.UserLevelsTbls.FirstOrDefault(p => p.LevelId == user.LevelId).LevelName;
                        int plan = db.TrafficPlanTbls.FirstOrDefault(p => p.PlanId == user.PlanId).TrafficPlan;
                        double memSize = user.MemorySize;

                        return new FormatedUser{ Login = login, Level = level, Plan = plan, MemorySize = memSize };
                    }
                }
                catch
                {
                    return null;
                }
        }

        public FilesTbl GetFullFileInformation(FilesTbl file)
        {
            //
            // Получение полной структуры файла с базы данных.
            // 
            // Метод возвращает структуру, содержащую все данные о файле, включая сам файл.
            //
            //

            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {
                    FilesTbl FullFile = db.FilesTbls.SingleOrDefault(p => p.FileId == file.FileId);

                    return FullFile;
                }
            }
            catch 
            {
                return null;
            }
        }

        public int BanUser(string UserName, string BanName)
        {
            //
            //Метод, банящий пользователя
            //  возрващает 1 - если такого пользователя не существует;
            //  возвращает 2 - если такой пользователь уже забанен
            //  возвращает 0 - операция проведена успешно
            //
            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {
                    UsersTbl user = db.UsersTbls.FirstOrDefault(p => p.UserId == UserName);
                    if (user == null)
                        return 0x01;

                    BanListTbl BanNote = new BanListTbl { UserId = user.UserId, BanId = db.BansTbls.FirstOrDefault(p => p.BanName == BanName).BanId };

                    if (db.BanListTbls.ToList().Count >= 1)
                        foreach (BanListTbl note in db.BanListTbls.ToList())
                        {
                            if (BanNote.UserId == note.UserId)
                                return 0x02;
                        }

                    db.Database.ExecuteSqlRaw("INSERT INTO BanList_tbl VALUES ({0}, {1})", BanNote.UserId, BanNote.BanId);
                    db.SaveChanges();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0x02;
            }
        }

        public int UnBanUser(string UserName)
        {
            //
            //Метод, банящий пользователя
            //  возрващает 1 - если такого пользователя не существует;
            //  возрващает 2 - если что-то пошло не так;
            //  возвращает 0 - операция проведена успешно
            //
            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {
                    UsersTbl user = db.UsersTbls.FirstOrDefault(p => p.UserId == UserName);
                    if (user == null)
                        return 0x01;

                    db.Database.ExecuteSqlRaw("DELETE BanList_tbl WHERE UserId = {0}", user.UserId);
                    db.SaveChanges();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0x02;
            }
        }

        public int UpgradeUser(string UserName, int Plan, string Level)
        {
            //
            //Метод, обновляющий пользователя
            //  возрващает 1 - если такого пользователя не существует;
            //  возрващает 2 - если что-то пошло не так;
            //  возвращает 0 - операция проведена успешно
            //
            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {
                    UsersTbl user = db.UsersTbls.FirstOrDefault(p => p.UserId == UserName);
                    if (user == null)
                        return 0x01;

                    user.LevelId = db.UserLevelsTbls.FirstOrDefault(p => p.LevelName == Level).LevelId;
                    user.PlanId = db.TrafficPlanTbls.FirstOrDefault(p => p.TrafficPlan == Plan).PlanId;

                    db.UsersTbls.Update(user);
                    db.SaveChanges();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0x02;
            }
        }

        public int AddFileType(string NewFileType)
        {
            //
            //Метод, добавляющий новый тип файла
            //  возрващает 1 - если такой файл существует;
            //  возрващает 2 - если что-то пошло не так;
            //  возвращает 0 - операция проведена успешно
            //

            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {
                    FileTypeTbl type = db.FileTypeTbls.FirstOrDefault(p => p.TypeName == NewFileType);
                    if (type != null)
                        return 0x01;

                    type = new FileTypeTbl { TypeName = NewFileType };
                    db.FileTypeTbls.Add(type);
                    db.SaveChanges();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0x02;
            }
        }

        public int AddNewCountry(string NewCountry)
        {
            //
            //Метод, добавляющий новую страну
            //  возрващает 1 - если такая страна существует;
            //  возрващает 2 - если что-то пошло не так;
            //  возвращает 0 - операция проведена успешно
            //

            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {
                    CountriesTbl country = db.CountriesTbls.FirstOrDefault(p => p.CountryName == NewCountry);
                    if (country != null)
                        return 0x01;

                    country = new CountriesTbl { CountryName = NewCountry };
                    db.CountriesTbls.Add(country);
                    db.SaveChanges();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0x02;
            }
        }

        public int AddNewTrafficPlan(int NewPlan)
        {
            //
            //Метод, добавляющий новый тарифный план
            //  возрващает 1 - если такой план уже существует;
            //  возрващает 2 - если что-то пошло не так;
            //  возвращает 0 - операция проведена успешно
            //

            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {
                    TrafficPlanTbl plan = db.TrafficPlanTbls.FirstOrDefault(p => p.TrafficPlan == NewPlan);
                    if (plan != null)
                        return 0x01;

                    plan = new TrafficPlanTbl { TrafficPlan = NewPlan };
                    db.TrafficPlanTbls.Add(plan);
                    db.SaveChanges();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0x02;
            }
        }

        public int AddNewBan(string NewBan)
        {
            //
            //Метод, добавляющий новую причину бана
            //  возрващает 1 - если такая причина уже существует;
            //  возрващает 2 - если что-то пошло не так;
            //  возвращает 0 - операция проведена успешно
            //

            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {
                    BansTbl ban = db.BansTbls.FirstOrDefault(p => p.BanName == NewBan);
                    if (ban != null)
                        return 0x01;

                    ban = new BansTbl { BanName = NewBan };
                    db.BansTbls.Add(ban);
                    db.SaveChanges();
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0x02;
            }
        }

        public bool IsBanned()
        {
            //
            //
            //Метод, возвращающий true или false  
            //  в зависимости от нахождения пользователя
            //  в таблице нарушителей.
            //

            try
            {
                using (sharefiledbContext db = new sharefiledbContext())
                {
                    BanListTbl Banned = db.BanListTbls.FirstOrDefault(p => p.UserId == this.user.UserId);

                    if (Banned == null)
                        return false;
                    else
                    {
                        BanReason = db.BansTbls.FirstOrDefault(p => p.BanId == Banned.BanId).BanName;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return true;
            }
        }

        //шифрование
        private static string Encrypt(string data)
        {
            using (var des = new TripleDESCryptoServiceProvider { 
                Mode = CipherMode.ECB, 
                Key = Encoding.ASCII.GetBytes("Ly!miiDjy*ebH$2X"), 
                Padding = PaddingMode.PKCS7 
            })

            using (var desEncrypt = des.CreateEncryptor())
            {
                var buffer = Encoding.UTF8.GetBytes(data);

                return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
        }

        private static string Decrypt(string data)
        {
            using (var des = new TripleDESCryptoServiceProvider { Mode = CipherMode.ECB, Key = Encoding.ASCII.GetBytes("Ly!miiDjy*ebH$2X"), Padding = PaddingMode.PKCS7 })
            using (var desEncrypt = des.CreateDecryptor())
            {
                var buffer = Convert.FromBase64String(data.Replace(" ", "+"));

                return Encoding.UTF8.GetString(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
        }
    }
}
