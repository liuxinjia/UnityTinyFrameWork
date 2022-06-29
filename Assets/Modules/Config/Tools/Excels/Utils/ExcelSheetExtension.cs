using System.Diagnostics;
using System;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Cr7Sund.Editor.Excels
{
    public static class ExcelSheetExtension
    {
        public static void CreateComment(this ICell cell, ISheet newSheet, string commentText)
        {
            // Create the drawing patriarch. This is the top level container for all shapes including cell comments.
            IDrawing patr = newSheet.CreateDrawingPatriarch();

            //anchor defines size and position of the comment in worksheet
            IComment comment = patr.CreateCellComment(new XSSFClientAnchor(0, 0, 0, 0, 4, 2, 6, 5));

            // set text in the comment
            comment.String = new XSSFRichTextString($"{commentText}");

            //set comment author.
            //you can see it in the status bar when moving mouse over the commented cell
            comment.Author = "Cr7Sund";

            // The first way to assign comment to a cell is via HSSFCell.SetCellComment method
            cell.CellComment = comment;
        }

        public static void CreateFont(this IRichTextString str, IWorkbook workbook)
        {
            IFont font = workbook.CreateFont();
            font.FontName = "Arial";
            font.FontHeightInPoints = 10;
            font.IsBold = true;
            font.Color = NPOI.HSSF.Util.HSSFColor.Red.Index;
            str.ApplyFont(font);
        }

        public static void SetRowStyle(this IRow row, IWorkbook workbook)
        {

            ICellStyle rowstyle = workbook.CreateCellStyle();
            rowstyle.FillForegroundColor = IndexedColors.Green.Index;
            rowstyle.FillPattern = FillPattern.Diamonds;

            row.RowStyle = rowstyle;
        }

        public static void SetCellStyle(this ICell cell, IWorkbook workbook)
        {
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = IndexedColors.Yellow.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cell.CellStyle = cellStyle;
        }
    }


}