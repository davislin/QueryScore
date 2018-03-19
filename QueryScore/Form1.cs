//using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Runtime.InteropServices;

namespace QueryScore
{
    public partial class Form1 : Form
    {

        private SQLiteConnection sqlite_conn;
        private SQLiteDataAdapter dta1;
        private SQLiteDataAdapter dta2;
        public Form1()
        {
            //this.WindowState = FormWindowState.Normal;
            //this.FormBorderStyle = FormBorderStyle.None;
            //SetWindowPos((int)this.Handle, HWND_TOP, 0, 0, GetSystemMetrics(SM_CXSCREEN), GetSystemMetrics(SM_CYSCREEN), SWP_SHOWWINDOW);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlite_conn = new SQLiteConnection("Data source=student.db");

            //String cmdText = "SELECT name, nmber FROM student_point.class";
            //MySqlCommand cmd = new MySqlCommand(cmdText, conn);
            //MySqlDataReader reader = cmd.ExecuteReader(); //execure the reader
            //while (reader.Read())
            //{
            //    _class = new string[reader.FieldCount / 2, 2];
            //    for (int i = 0; i < reader.FieldCount; i=i+2)
            //    {
            //        string s = reader.GetString(i);
            //        _class[i, 0] = s;
            //        string n = reader.GetString(i+1);
            //        _class[i, 1] = n;
            //        cbxClass.Items.Add(s);
            //        Console.Write(s + "\t" + n + "\t");
            //    }
            //    cbxClass.SelectedIndex = 0;
            //    Console.Write("\n");
            //}

            ////使用 MySqlDataAdapter 查詢資料，並將結果存回 DataSet 當做名為 test1 的 DataTable
            //string sql = "SELECT * FROM student_point.class";
            //MySqlDataAdapter dataAdapter1 = new MySqlDataAdapter(sql, conn);
            //dataAdapter1.Fill(dataSet, "test1");

            //// test1 的 DataTable
            //DataTable dataTable = dataSet.Tables["test1"];

            ////列出 test1 的第 1 筆資料
            //Console.WriteLine("name={0} , nmber={1}", dataTable.Rows[0]["name"], dataTable.Rows[0]["nmber"]);

            ////列出 test1 的總筆數
            //Console.WriteLine("總筆數:{0}", dataTable.Rows.Count);

            ////逐筆列出 test1 的資料
            //foreach (DataRow row in dataTable.Rows)
            //{
            //    cbxClass.Items.Add(row["name"]);
            //    Console.WriteLine("id={0},name={1},nmber={2}", row["id"], row["name"], row["nmber"]);
            //}

            //使用 MySqlDataAdapter 查詢資料，並將結果存回 DataSet 當做名為 test2 的 DataTable
            //dataAdapter2 = new MySqlDataAdapter("SELECT * FROM student_point.pointlog", conn);
            //dataAdapter2.Fill(dataSet, "test2");

            //指定資料來源
            //dataGridView1.DataSource = dataSet;

            ////指定列出 test1 DataTable 內容
            //dataGridView1.DataMember = "test2";
            //dataGridView1.Columns["id"].HeaderText = "編號";
            //dataGridView1.Columns["number"].HeaderText = "學號";
            //dataGridView1.Columns["stu_class"].HeaderText = "班級";
            //dataGridView1.Columns["name"].HeaderText = "姓名";
            //dataGridView1.Columns["date"].HeaderText = "日期";
            //dataGridView1.Columns["pt_case"].HeaderText = "獎勵事由";
            //dataGridView1.Columns["point"].HeaderText = "獎勵點數";
            //dataGridView1.Columns["exchange_info"].HeaderText = "兌換登記";
            //cbxClass.SelectedIndex = 0;


            //Console.ReadLine();
            //conn.Close();
            labClass.Text = "";
            labName.Text = "";
            tbxNM.Select();

        }

        private void GetNewData()
        {
            // Open
            sqlite_conn.Open();

            dataSet.Clear();

            dta1 = new SQLiteDataAdapter("select class.name AS CLASS, IFNULL(student.name,'查無此人')NAME, IFNULL(SUM(pointlog.point),0)POINT FROM student left outer join pointlog ON student.number = pointlog.number left outer join class ON student.stu_class = class.number Where student.number == " + tbxNM.Text, sqlite_conn);

            dta1.Fill(dataSet, "student");
            //// 執行查詢塞入 sqlite_datareader

            dta2 = new SQLiteDataAdapter("select date,point,pt_case FROM pointlog Where pointlog.number == " + tbxNM.Text, sqlite_conn);

            dta2.Fill(dataSet, "pointlog");

            //}

            //結束
            sqlite_conn.Close();
        }

        //private void cbxClass_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //dataAdapter2.SelectCommand = new MySqlCommand( string.Format("SELECT * FROM student_point.pointlog Where stu_class = '{0}'", cbxClass.SelectedItem.ToString()));
        //    //dataAdapter2.Fill(dataSet, "test2");
        //    DataTable dt = dataSet.Tables["test2"];
        //    //過濾後的 dataGridView
        //    DataView view = new DataView(dt);
        //    view.RowFilter = string.Format("stu_class = '{0}'", cbxClass.SelectedItem.ToString());
        //    DataTable table = view.ToTable();
        //    dataGridView2.DataSource = table;
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            GetNewData();

            dataGridView2.DataSource = dataSet.Tables["pointlog"];
            dataGridView2.Columns["date"].HeaderText = "日期";
            dataGridView2.Columns["point"].HeaderText = "變動點數";
            dataGridView2.Columns["pt_case"].HeaderText = "獎勵或兌換說明";
            //計算總點數
            if(dataSet.Tables["student"].Rows.Count > 0)
            {
                labClass.Text = dataSet.Tables["student"].Rows[0]["CLASS"].ToString();
                labName.Text = dataSet.Tables["student"].Rows[0]["NAME"].ToString();
                labPoint.Text = dataSet.Tables["student"].Rows[0]["POINT"].ToString();
            }
            else
            {
                labClass.Text ="查無資料";
                labName.Text = ""; 
                labPoint.Text = "";
            }

        }

        private void tbxNM_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(13))
            {
                tbxNM.SelectAll();
                button1_Click(null,null);

            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void tbxNM_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void Form1_Deactivate(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.ControlBox = true;
            //this.WindowState = FormWindowState.Maximized;
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }
    }
}
