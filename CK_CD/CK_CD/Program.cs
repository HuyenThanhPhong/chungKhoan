using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CK_CD
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static SqlConnection conn = new SqlConnection();//-> cầu nối để kết nối vs DB, tồn tại suyên sốt chương trình
        public static String connstr; //kết nối site con
        public static SqlDataReader myReader;
        public static String servername = "DATLE\\SERVER";

        public static String database = "CHUNGKHOANG";

        ///////
        public static String remotelogin = "sa";
        public static String remotepassword = "123456";

        public static int KetNoi()
        {

            if (Program.conn != null && Program.conn.State == ConnectionState.Open)
                Program.conn.Close();

            try
            {
                Program.connstr = "Data Source=" + Program.servername + ";Initial Catalog=" +
                      Program.database + ";User ID=" +
                      remotelogin + ";password=" + Program.remotepassword;
                Program.conn.ConnectionString = Program.connstr; // bắt đầu kết nối
                Program.conn.Open();
                return 1;
            }

            catch (Exception e)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu.\nBạn xem lại user name và password.\n " + e.Message, "", MessageBoxButtons.OK);
                return 0;
            }
        }
        //sp select view từ csdl, trả về 1 là reader/ 2 là table
        //data reader : ưu: truy vấn nhanh, nhiều dòng chỉ cho đi xuống, k đi ngược lại => dùng cho báo cáo || nhược: chỉ xem
        //data table -> cho phép sửa và cập nhật lại sql
        public static SqlDataReader ExecSqlDataReader(String strLenh)
        {

            SqlDataReader myreader;
            SqlCommand sqlcmd = new SqlCommand(strLenh, Program.conn); // gọi sp hoặc chạy truy vấn, chuỗi lệnh muốn chạy và chạy trên kết nối nào
            sqlcmd.CommandType = CommandType.Text;
            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            try
            {
                myreader = sqlcmd.ExecuteReader(); return myreader;

            }
            catch (SqlException ex)
            {
                Program.conn.Close();
                //MessageBox.Show(ex.Message);
                return null;
            }
        }


        public static DataTable ExecSqlDataTable(String cmd)
        {
            DataTable dt = new DataTable();
            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd, conn);
            da.Fill(dt);
            conn.Close();
            return dt;
        }

        public static int ExecSqlNonQuery(string cmd)
        {
            using (SqlConnection connection = new SqlConnection(Program.connstr))
            {
                SqlCommand command = new SqlCommand(cmd, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
                return 1;
            }
        }
        [STAThread]
        static void Main()
        {
            int i = KetNoi();
                        Console.WriteLine("KẾT QUẢ: "+i);
               
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
            
        }
    }
}
