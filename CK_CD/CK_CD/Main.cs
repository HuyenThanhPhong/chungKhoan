using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CK_CD
{
    public partial class Main : Form
    {
        int changeCount = 0;
        String tableName = "BANGGIATRUCTUYEN";

        private SqlConnection connection = null;
        private SqlCommand command = null;
        private DataSet dataSet = null;

        

        public Main()
        {
            InitializeComponent();
        }

        private void lENHDATBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsLENHDAT.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private string GetSQL()
                {
                    return "SELECT MACP, " +
                        "GIAMUA1, KLMUA1, GIAMUA2, KLMUA2, GIAMUA3, KLMUA3," +
                        " GIAKL, KLKL, TANGGIAMKL," +
                        " GIABAN1, KLBAN1, GIABAN2, KLBAN2, GIABAN3, KLBAN3," +
                        " TONGKL "+
                        "FROM DBO.BANGGIATRUCTUYEN";
                }

        private void Main_Load(object sender, EventArgs e)
        {
            
            
            this.lENHDATTableAdapter.Fill(this.DS.LENHDAT);
            this.bANGGIATRUCTUYENTableAdapter.Fill(this.DS.BANGGIATRUCTUYEN);
            this.v_DS_MACPTableAdapter.Fill(this.DS.V_DS_MACP);
            this.lENHKHOPTableAdapter.Fill(this.DS.LENHKHOP);

            SqlClientPermission sqlClientPermission = new SqlClientPermission(System.Security.Permissions.PermissionState.Unrestricted);
            sqlClientPermission.Demand();

            BatDau();

        }
        public void OnDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            ISynchronizeInvoke i = (ISynchronizeInvoke)this;
            if (i.InvokeRequired)
            {
                OnChangeEventHandler tempDelegate = new OnChangeEventHandler(OnDependencyChange);
                object[] args = new[] { sender, e };
                i.BeginInvoke(tempDelegate, args);
                return;
            }
            SqlDependency sqlDependency = sender as SqlDependency;
            sqlDependency.OnChange -= OnDependencyChange;
            GetDataFromSQL();
        }


        private void GetDataFromSQL()
        {
            dataSet.Clear();

            command.Notification = null;

            SqlDependency dependency = new SqlDependency(command);
            dependency.OnChange += OnDependencyChange;

            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(dataSet, tableName);

                this.bANGGIATRUCTUYENGridControl.DataSource = dataSet;
                this.bANGGIATRUCTUYENGridControl.DataMember = tableName;
            }

        }


        private void BatDau()
        {
            changeCount = 0;

            SqlDependency.Stop(Program.connstr);

            try
            {
                SqlDependency.Start(Program.connstr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "THÔNG BÁO", MessageBoxButtons.OK);
                return;
            }

            if (connection == null)
            {
                connection = new SqlConnection(Program.connstr);
                connection.Open();
            }

            if (command == null)
            {
                command = new SqlCommand(GetSQL(), connection);
            }

            if (dataSet == null)
            {
                dataSet = new DataSet();
            }

            GetDataFromSQL();
        }

        private int checkExitSymbol()
        {
            String sql = "SELECT MACP FROM BANGGIATRUCTUYEN WHERE MACP = '" + cmbMACP.Text.ToString().Trim() + "'";
            Program.myReader = Program.ExecSqlDataReader(sql);
            while (Program.myReader.Read() == false)
            {
                Program.myReader.Close();
                return 0;
            }
            Program.myReader.Close();
            return 1;
        }

        private void btnDatLenh_Click(object sender, EventArgs e)
        {
            //kiểm tra rỗng và điều kiện input
            if (cmbMACP.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa nhập mã cổ phiếu!", "THÔNG BÁO", MessageBoxButtons.OK);
                cmbMACP.Focus();
                return;
            }

            /*if (checkExitSymbol() == 0)
            {
                MessageBox.Show("Mã cổ phiếu này chưa tồn tại trên sàn giao dịch!", "THÔNG BÁO", MessageBoxButtons.OK);
                cmbMACP.Focus();
                return;
            }
            else { }*/

            if (txtGIADAT.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa nhập giá đặt!", "THÔNG BÁO", MessageBoxButtons.OK);
                txtGIADAT.Focus();
                return;
            }
            if (Int32.Parse(txtGIADAT.Text.Trim()) < 0)
            {
                MessageBox.Show("Giá đặt phải lớn hơn 0!", "THÔNG BÁO", MessageBoxButtons.OK);
                txtGIADAT.Focus();
                return;
            }
            if (txtSL.Text.Trim() == "")
            {
                MessageBox.Show("Bạn chưa nhập số lượng đặt!", "THÔNG BÁO", MessageBoxButtons.OK);
                txtSL.Focus();
                return;
            }
            if (Int32.Parse(txtSL.Text.Trim()) < 0)
            {
                MessageBox.Show("Số lượng đặt phải lớn hơn 0!", "THÔNG BÁO", MessageBoxButtons.OK);
                txtSL.Focus();
                return;
            }
            try
            {
                String loaiGD = "M";
                if (cmbLoaiGD.SelectedIndex == 1) loaiGD = "B";
                String spKhopLenh = "EXEC SP_KHOPLENH_LO '" + cmbMACP.Text.Trim() + "', '" + DateTime.Now + "', '"
                    + loaiGD + "', '" + txtSL.Text.Trim() + "', '" + txtGIADAT.Text.Trim() + "'";

                Program.myReader = Program.ExecSqlDataReader(spKhopLenh);
                Program.myReader.Close();
                DialogResult rsSuccess = MessageBox.Show("Đặt lệnh thành công, bạn có muốn thực hiện giao dịch khác?", "", MessageBoxButtons.YesNo);
                if (DialogResult.Yes == rsSuccess)
                {
                    cmbMACP.SelectedIndex = 0;
                    cmbLoaiGD.SelectedIndex = 0;
                    txtGIADAT.Text = "";
                    txtSL.Text = "";
                    cmbMACP.Focus();
                    this.lENHDATTableAdapter.Fill(this.DS.LENHDAT);
                    this.v_DS_MACPTableAdapter.Fill(this.DS.V_DS_MACP);
                    this.lENHKHOPTableAdapter.Fill(this.DS.LENHKHOP);

                }
                else
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                DialogResult rsError = MessageBox.Show("Đã có lỗi xảy ra khi đặt lệnh, bạn có muốn thử lại?\n " + ex.Message, "", MessageBoxButtons.RetryCancel);
                if (DialogResult.Retry == rsError)
                {
                    //cmbMACP.SelectedIndex = 0;
                    cmbLoaiGD.SelectedIndex = 0;
                    txtGIADAT.Text = "";
                    txtSL.Text = "";
                    cmbMACP.Focus();
                }
                else
                {
                    Close();
                }
                return;
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            //cmbMACP.SelectedIndex = 0;
            cmbLoaiGD.SelectedIndex = 0;
            txtGIADAT.Text = "";
            txtSL.Text = "";
        }
    }
}
