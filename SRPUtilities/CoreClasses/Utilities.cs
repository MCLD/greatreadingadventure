using System;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STG.SRP.Utilities
{
    public class Utilities
    {

        // This is a helper method used to determine the index of the
        // column being sorted. If no column is being sorted, -1 is returned.
        public static int GetSortColumnIndex(String strCol, GridView _gv)
        {
            foreach (DataControlField field in _gv.Columns)
            {
                if (field.SortExpression == strCol)
                {
                    return _gv.Columns.IndexOf(field);
                }
            }

            return -1;
        }

        public static void AddSortImage(GridViewRow headerRow, GridView _gv, string m_strSortExp, SortDirection m_SortDirection)
        {
            Int32 iCol = GetSortColumnIndex(m_strSortExp, _gv);
            if (-1 == iCol)
            {
                return;
            }
            // Create the sorting image based on the sort direction.
            Image sortImage = new Image();
            if (SortDirection.Ascending == m_SortDirection)
            {
                sortImage.ImageUrl = "~/ControlRoom/Images/dwn.gif";
                sortImage.AlternateText = "Ascending Order";
            }
            else
            {
                sortImage.ImageUrl = "~/ControlRoom/Images/up.gif";
                sortImage.AlternateText = "Descending Order";
            }

            // Add the image to the appropriate header cell.
            headerRow.Cells[iCol].Controls.Add(sortImage);
        }

        public static DataTable ConvertArrayToTable(Array myList)
        {
            DataTable dt = new DataTable();
            if (myList.Length > 0)
            {
                PropertyInfo[] propInfos = myList.GetValue(0).GetType().GetProperties();

                foreach (PropertyInfo propInfo in propInfos)
                {
                    dt.Columns.Add(propInfo.Name, propInfo.PropertyType);
                }

                foreach (object tempObject in myList)
                {
                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < propInfos.Length; i++)
                    {
                        dr[i] = propInfos[i].GetValue(tempObject, null);
                    }

                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        public static void gv_RowCreated(object sender, GridViewRowEventArgs e, string m_strSortExp, SortDirection m_SortDirection)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (String.Empty != m_strSortExp)
                {
                    Utilities.AddSortImage(e.Row, (GridView)sender, m_strSortExp, m_SortDirection);
                }
            }
        }
    }
}
