using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Threading;

namespace ThreatDatabase
{
    class Database : DbContext
    {
        public static string DateBaseFile = App.Directory + @"\Database.mdf";

        private readonly string XmlPath = App.Directory + @"\threats.xlsx";
        private readonly string XmlUrl = "https://bdu.fstec.ru/documents/files/thrlist.xlsx";

        public DbSet<Threat> Threats { get; set; }

        public Database() : base("Database") { }

        public void Clear()
        {
            foreach (var t in Threats)
            {
                Entry(t).State = EntityState.Deleted;
            }
            SaveChanges();
        }

        public Task<Queue<ThreatChange>> Update()
        {
            try
            {
                new WebClient().DownloadFile(XmlUrl, XmlPath);

                var changes = new Queue<ThreatChange>();

                using (var excel = new ExcelReader(XmlPath))
                {
                    excel.OpenTable(0);
                    int rowsCount = excel.GetRowsCount();

                    var allUpdatedID = new List<int>();
                    Threat databaseThreat, updatedThreat;
                    for (int i = 2; i < rowsCount; i++)
                    {
                        updatedThreat = new Threat()
                        {
                            Number = (int)excel.TryRead<double>(i, 0),
                            Name = excel.TryRead<string>(i, 1),
                            Discription = excel.TryRead<string>(i, 2),
                            Source = excel.TryRead<string>(i, 3),
                            Object = excel.TryRead<string>(i, 4),
                            IsPrivacyViolation = excel.TryRead<int>(i, 5) == 1,
                            IsIntegrityViolation = (int)excel.TryRead<double>(i, 6) == 1,
                            IsAccessibilityViolation = (int)excel.TryRead<double>(i, 7) == 1,
                            LastUpdateTime = excel.TryRead<DateTime>(i, 9).ToBinary()
                        };

                        allUpdatedID.Add(updatedThreat.Number);

                        databaseThreat = Threats.FirstOrDefault(x => x.Number == updatedThreat.Number);
                        if (databaseThreat != null)
                        {
                            if (databaseThreat.LastUpdateTime == updatedThreat.LastUpdateTime)
                            {
                                continue;
                            }
                            else
                            {
                                changes.Enqueue(new ThreatChange(databaseThreat.Clone(), updatedThreat));
                                updatedThreat.ID = databaseThreat.ID;
                                Entry(databaseThreat).CurrentValues.SetValues(updatedThreat);
                                continue;
                            }
                        }
                        else
                        {
                            changes.Enqueue(new ThreatChange(null, updatedThreat));
                            Threats.Add(updatedThreat);
                            continue;
                        }
                    }
                    
                    foreach (var item in Threats.Where(x => !allUpdatedID.Contains(x.Number)))
                    {
                        Entry(item).State = EntityState.Deleted;
                        changes.Enqueue(new ThreatChange(item, null));
                    }
                }

                SaveChanges();

                File.Delete(XmlPath);

                return Task.FromResult(changes);
            }
            catch (Exception e)
            {
                return Task.FromException<Queue<ThreatChange>>(e);
            }

        }


    }
}
