﻿using Microsoft.Office.Interop.Word;
using Models;
using System;
using System.Collections.Generic;

namespace Desktop.Tools
{
    public class Word
    {
        public Application Application { get; }

        public Word()
        {
            Application = new Application
            {
                Visible = false
            };
        }

        public Document CreatePaymentsDocument()
        {
            Document doc = CreateDocument();

            doc.Content.Font.Size = 13f;
            doc.Content.Font.Name = "Calibri";
            return doc;
        }

        public Document CreateDocument()
        {
            return Application.Documents.Add();
        }

        public Document OpenDocument(string path, bool isReadonly = false)
        {
            return Application.Documents.Open(path, ReadOnly: isReadonly);
        }

        public void SaveAs(Document doc, string path)
        {
            doc.SaveAs(path);
        }

        public void Save(Document doc)
        {
            doc.Save();
        }

        public void CopyDocument(string sourceFile, string copiedFile)
        {
            Document doc = Application.Documents.Open(sourceFile, ReadOnly: true);
            doc.SaveAs(copiedFile);
            doc.Close();
        }

        public void PrintOut(Document doc)
        {
            doc.PrintOut();
        }

        public void CloseDocument(Document doc)
        {
            doc.Close();
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void FormationPaymentsDocument(Document doc, List<Payment> payments)
        {
            CreateTemplatePaymentsTable(doc);

            string name = string.IsNullOrEmpty(payments[0].FlatOwner) ?
                "\"Имя владельца\"" : payments[0].FlatOwner;
            doc.Tables[1].Cell(1, 1).Range.Text =
                $"Платежи для {name}  (лицевой счет: {payments[0].AccountId})";

            doc.Tables[1].Rows[3].Range.Cut();
            foreach (var payment in payments)
            {
                FormingPayment(doc, payment);
            }

            Microsoft.Office.Interop.Word.Range content = doc.Content;
            content.SetRange(content.End, content.End);
            content.Text = "\n\nБухгалтер ЧП СК Комфорт Одесса              Крепак Н.В.";
        }

        private void FormingPayment(Document doc, Payment payment)
        {
            Paste(doc, false);
            WordReplace(doc, "{FWR}", payment.ForWer);
            WordReplace(doc, "{FWTR}", payment.ForWater);
            WordReplace(doc, "{FH}", payment.ForHeating);
            WordReplace(doc, "{TT}", payment.Total);
            WordReplace(doc, "{MT}", payment.Month);
            WordReplace(doc, "{YR}", payment.Year);
        }

        public void FormationFlayer(Document doc, IList<object> info, IList<object> rates, string house)
        {
            Paste(doc);
            WordReplace(doc, "{NM}", info[2]);
            WordReplace(doc, "{AD}", info[1]);
            WordReplace(doc, "{MT}", rates[6]);
            WordReplace(doc, "{FA}", $"ул. Пишоновская, {house.Replace('_', '/')} кв. {info[0]}");
            int ms = (int)GetNumberValue(rates[7]);
            int me = (int)GetNumberValue(rates[8]);
            WordReplace(doc, "{MS}", ms > 9 ? ms.ToString() : "0" + ms);
            WordReplace(doc, "{ME}", me > 9 ? me.ToString() : "0" + me);
            WordReplace(doc, "{HSS}", Math.Round(GetNumberValue(info[3]) - GetNumberValue(info[4]), 2)); // debet - credit
            WordReplace(doc, "{CHV}", string.IsNullOrEmpty(info[6].ToString()) ? "-" : GetNumberValue(info[7]).ToString()); // - or value
            WordReplace(doc, "{PHV}", string.IsNullOrEmpty(info[6].ToString()) ? "-" : GetNumberValue(info[8]).ToString()); // - or value
            WordReplace(doc, "{HV}", string.IsNullOrEmpty(info[6].ToString()) ? "-" : GetNumberValue(info[9]).ToString()); // - or value
            WordReplace(doc, "{HR}", string.IsNullOrEmpty(info[6].ToString()) ? rates[1] : rates[2]); // central or custom
            WordReplace(doc, "{FH}", Math.Round(GetNumberValue(info[10]) - GetNumberValue(info[11]), 2)); // forHeating - privileges
            WordReplace(doc, "{HP}", Math.Round(GetNumberValue(info[13]) + GetNumberValue(info[14]), 2)); // cashbox + bank
            WordReplace(doc, "{HSE}", Math.Round(GetNumberValue(info[15]) - GetNumberValue(info[16]), 2)); // debet - credit
            WordReplace(doc, "{WRSS}", Math.Round(GetNumberValue(info[18]) - GetNumberValue(info[19]), 2)); // debet - credit
            WordReplace(doc, "{WRR}", (int)GetNumberValue(info[0]) < 7 ? rates[4] : rates[3]); // special or general
            WordReplace(doc, "{FWR}", Math.Round(GetNumberValue(info[21]) - GetNumberValue(info[27]), 2)); // forWer - privileges
            WordReplace(doc, "{WRP}", Math.Round(GetNumberValue(info[29]) + GetNumberValue(info[30]) - GetNumberValue(info[25]), 2)); // cashbox + bank - forWater
            WordReplace(doc, "{WRSE}", Math.Round(GetNumberValue(info[31]) - GetNumberValue(info[32]), 2)); // debet - credit
            WordReplace(doc, "{CWV}", GetNumberValue(info[22])); // current water value
            WordReplace(doc, "{PWV}", GetNumberValue(info[23])); // prev water value
            WordReplace(doc, "{WV}", GetNumberValue(info[24]));  // water value
            WordReplace(doc, "{WTR}", rates[0]); // water rate
            WordReplace(doc, "{FWT}", GetNumberValue(info[25])); // for water
            WordReplace(doc, "{WTP}", GetNumberValue(info[25])); // water payment
            WordReplace(doc, "{GSS}", "-");
            WordReplace(doc, "{GR}", "-");
            WordReplace(doc, "{FG}", "-");
            WordReplace(doc, "{GP}", "-");
            WordReplace(doc, "{GSE}", "-");
        }

        /// <summary>
        /// Convert object to double
        /// </summary>
        /// <param name="obj">Convertible object</param>
        /// <returns>Double value</returns>
        private static double GetNumberValue(object obj)
        {
            return string.IsNullOrEmpty(obj.ToString()) ? 0.0 : double.Parse(obj.ToString());
        }

        private void CreateTemplatePaymentsTable(Document doc)
        {
            Microsoft.Office.Interop.Word.Range tableRange = doc.Content;
            tableRange.SetRange(tableRange.Start, tableRange.Start);
            Table table = doc.Tables.Add(tableRange, 1, 6);

            table.Rows[1].Height = 19f;
            table.Range.Font.Size = 12f;
            table.Range.Paragraphs.SpaceAfter = 0f;
            table.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
            table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

            table.Cell(1, 1).Width = 134f;
            table.Cell(1, 2).Width = 71f;
            table.Cell(1, 3).Width = 85f;
            table.Cell(1, 4).Width = 64f;
            table.Cell(1, 5).Width = 71f;
            table.Cell(1, 6).Width = 52f;

            table.Cell(1, 1).Range.Text = "{FWR}";
            table.Cell(1, 2).Range.Text = "{FWTR}";
            table.Cell(1, 3).Range.Text = "{FH}";
            table.Cell(1, 4).Range.Text = "{TT}";
            table.Cell(1, 5).Range.Text = "{MT}";
            table.Cell(1, 6).Range.Text = "{YR}";

            table.Cell(1, 1).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            table.Cell(1, 2).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            table.Cell(1, 3).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            table.Cell(1, 4).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            table.Cell(1, 5).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            table.Cell(1, 6).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            //Table table = doc.Tables[1];
            table.Rows.Add(table.Rows[1]);
            table.Rows[1].Range.Font.Bold = -1;
            table.Cell(1, 1).Range.Text = "За содержание дома";
            table.Cell(1, 2).Range.Text = "За воду";
            table.Cell(1, 3).Range.Text = "За отопление";
            table.Cell(1, 4).Range.Text = "Всего";
            table.Cell(1, 5).Range.Text = "Месяц";
            table.Cell(1, 6).Range.Text = "Год";

            table.Rows.Add(table.Rows[1]);
            for (int i = 0; i < 5; i++)
            {
                table.Cell(1, 1).Merge(table.Cell(1, 2));
            }
        }

        /// <summary>
        /// Paste info to word document
        /// </summary>
        /// <param name="doc">Word document</param>
        /// <param name="start">Need to insert at the beginning or at the end of document</param>
        private void Paste(Document doc, bool start = true)
        {
            Microsoft.Office.Interop.Word.Range range = doc.Content;
            if (start)
            {
                range.SetRange(range.Start, range.Start);
            }
            else
            {
                range.SetRange(range.End, range.End);
            }
            range.Select();
            Application.Selection.Paste();
        }

        /// <summary>
        /// Replace template string
        /// </summary>
        /// <param name="doc">Word document</param>
        /// <param name="replace">Template string</param>
        /// <param name="replaceWith">Text to replace</param>
        private void WordReplace(Document doc, string replace, object replaceWith)
        {
            doc.Content.Find.Execute(FindText: replace, ReplaceWith: replaceWith);
        }
    }
}
