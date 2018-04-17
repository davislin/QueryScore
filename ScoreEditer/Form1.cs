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
using System.Data.OleDb;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace ScoreEditer
{
    public partial class Form1 : Form
    {
        private SQLiteConnection sqlite_conn;
        private SQLiteDataAdapter dataAdapter1;
        private SQLiteDataAdapter dataAdapter2;
        private SQLiteDataAdapter dataAdapter3;
        private SQLiteDataAdapter dataAdapter4;
        private SQLiteCommandBuilder Builder1;
        private SQLiteCommandBuilder Builder2;
        private SQLiteCommandBuilder Builder3; 
        private bool IsFindStudent;

        public Form1()
        {
            InitializeComponent();
            if (File.Exists("student.db"))
            {
                sqlite_conn = new SQLiteConnection("Data source=student.db");
            }
            else
            {
                MessageBox.Show("歡迎使用好讚點數系統!!\r\n由於您目前沒有任何資料所以自動開啟一個新的資料庫");
                SQLiteConnection.CreateFile("student.db");
                sqlite_conn = new SQLiteConnection("Data source=student.db");
                sqlite_conn.Open();

                string sql = "CREATE TABLE class ( `number` INTEGER, `name` TEXT, PRIMARY KEY(`number`) )";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite_conn);
                command.ExecuteNonQuery();
                string sql1 = "CREATE TABLE pointlog ( `id` INTEGER, `number` INTEGER, `stu_class` TEXT, `name` TEXT, `date` TEXT, `point` INTEGER, `pt_case` TEXT, PRIMARY KEY(`id`) )";
                SQLiteCommand command1 = new SQLiteCommand(sql1, sqlite_conn);
                command1.ExecuteNonQuery();
                string sql2 = "CREATE TABLE student ( `number` INTEGER, `stu_class` INTEGER, `name` TEXT, PRIMARY KEY(`number`) )";
                SQLiteCommand command2 = new SQLiteCommand(sql2, sqlite_conn);
                command2.ExecuteNonQuery();

                sqlite_conn.Close();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = @"好讚點數管理系統 v" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

            GetNewData();
        }

        private void GetNewData()
        {
            dataSet.Clear();
            //MySqlCommand command = conn.CreateCommand();
            sqlite_conn.Open();

            //##########班級資料
            dataAdapter1 = new SQLiteDataAdapter("SELECT * FROM class order by number", sqlite_conn);
            dataAdapter1.Fill(dataSet, "class");

            //指定資料來源
            dataGridView1.DataSource = dataSet;

            //指定列出 test1 DataTable 內容
            dataGridView1.DataMember = "class";
            dataGridView1.Columns["number"].HeaderText = "代號";
            dataGridView1.Columns["name"].HeaderText = "班級";
            //dataGridView1.Sort(dataGridView1.Columns["number"], ListSortDirection.Ascending);

            dataSet.Tables["class"].PrimaryKey = new DataColumn[] { dataSet.Tables["class"].Columns["number"] };

            //添加班級到按鈕
            for(int i=0;i<dataSet.Tables["class"].Rows.Count;i++)
            {
                cbxClass.Items.Add(dataSet.Tables["class"].Rows[i]["name"]);
            }
            cbxClass.SelectedIndex = dataSet.Tables["class"].Rows.Count > 0 ? 0 : -1;

            //##########點數紀錄資料
            dataAdapter2 = new SQLiteDataAdapter("SELECT * FROM pointlog order by id DESC", sqlite_conn);
            dataAdapter2.Fill(dataSet, "pointlog");

            //指定資料來源
            dataGridView2.DataSource = dataSet;

            //指定列出 test1 DataTable 內容
            dataGridView2.DataMember = "pointlog";
            //dataGridView2.Columns["id"].HeaderText = "編號";
            dataGridView2.Columns["id"].Visible = false;
            dataGridView2.Columns["number"].HeaderText = "學號";
            dataGridView2.Columns["stu_class"].HeaderText = "班級";
            dataGridView2.Columns["name"].HeaderText = "姓名";
            dataGridView2.Columns["date"].HeaderText = "日期";
            dataGridView2.Columns["pt_case"].HeaderText = "獎勵事由";
            dataGridView2.Columns["point"].HeaderText = "獎勵點數";
            //dataGridView2.Columns["exchange_info"].HeaderText = "兌換登記";

            //##########學生資料
            dataAdapter3 = new SQLiteDataAdapter("SELECT stu_class AS 班級,number AS 學號,name AS 姓名 FROM student", sqlite_conn);
            dataAdapter3.Fill(dataSet, "student");

            //指定資料來源
            dataGridView3.DataSource = dataSet;

            //指定列出 test1 DataTable 內容
            dataGridView3.DataMember = "student";
            //dataGridView3.Columns["stu_class"].HeaderText = "班級";
            //dataGridView3.Columns["number"].HeaderText = "學號";
            //dataGridView3.Columns["name"].HeaderText = "姓名";
            dataGridView3.Sort(dataGridView3.Columns["學號"], ListSortDirection.Ascending);


            dataSet.Tables["student"].PrimaryKey = new DataColumn[] { dataSet.Tables["student"].Columns["學號"] };

            Builder1 = new SQLiteCommandBuilder(dataAdapter1);
            Builder2 = new SQLiteCommandBuilder(dataAdapter2);
            Builder3 = new SQLiteCommandBuilder(dataAdapter3);

            //##########統整資料
            dataAdapter4 = new SQLiteDataAdapter("select class.name AS CLASS, student.number,student.name, IFNULL(SUM(pointlog.point),0)POINT FROM student left outer join pointlog ON student.name = pointlog.name left outer join class ON student.stu_class = class.number group by student.name ORDER BY student.number", sqlite_conn);
            dataAdapter4.Fill(dataSet, "Total");

            //指定資料來源
            dataGridView4.DataSource = dataSet;

            //指定列出 test1 DataTable 內容
            dataGridView4.DataMember = "Total";
            dataGridView4.Columns["CLASS"].HeaderText = "班級";
            dataGridView4.Columns["number"].HeaderText = "學號";
            dataGridView4.Columns["name"].HeaderText = "姓名";
            dataGridView4.Columns["POINT"].HeaderText = "目前點數";
            //dataGridView4.Sort(dataGridView4.Columns["number"], ListSortDirection.Ascending);

            sqlite_conn.Close();

            tssMessage.Text = "讀取資料庫成功....";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataAdapter1.Update(dataSet, "class");
            dataSet.AcceptChanges();
            tssMessage.Text = "班級資料已上傳完成!";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataAdapter2.Update(dataSet, "pointlog");
            dataSet.AcceptChanges();
            tssMessage.Text = "點數資料已上傳完成!";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataAdapter3.Update(dataSet.Tables["student"]); 
            dataSet.AcceptChanges();
            tssMessage.Text = "學生資料已上傳完成!";
        }

        private void cbxClass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(13))
            {
                if (this.ActiveControl == btnImport)
                {
                    btnImport_Click(null, null);
                }
                else
                {
                    this.SelectNextControl(this.ActiveControl, true, true, true, false); //跳下一個元件
                }

            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (tbxNumber.Text != "")
            {
                DataRow newRow = dataSet.Tables["pointlog"].NewRow();
                //newRow["id"] = dataSet.Tables["pointlog"].Rows.Count + 1;
                newRow["number"] = Int32.Parse(tbxNumber.Text);
                newRow["stu_class"] = labClass.Text;
                newRow["name"] = labName.Text;
                newRow["date"] = dtPDate.Value;
                newRow["point"] = nmUDPoint.Value;
                newRow["pt_case"] = tbxPT_case.Text;
                dataSet.Tables["pointlog"].Rows.Add(newRow);
                dataAdapter2.Update(dataSet, "pointlog");
                dataSet.AcceptChanges();
                tbxNumber.Focus();
                tbxNumber.SelectAll();
                tssMessage.Text = "好讚點數資料已新增完成!";
            }
            else
            {
                MessageBox.Show("沒有輸入任何資料");
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            labName.Text = "查無此人";
            labClass.Text = "";
            IsFindStudent = false;
            if(tbxNumber.Text.Length > 0)
            {
                DataRow Row_temp = dataSet.Tables["student"].Rows.Find(tbxNumber.Text);
                if (Row_temp != null)
                {
                    labName.Text = Row_temp["姓名"].ToString();
                    DataRow Row_temp2 = dataSet.Tables["class"].Rows.Find(Row_temp["班級"]);
                    labClass.Text = Row_temp2["name"].ToString();
                    IsFindStudent = true;
                }

                if (IsFindStudent)
                {
                    btnImport.Enabled = true;
                }
                else
                {
                    btnImport.Enabled = false;
                }
            }

        }

        private void TMIrefresh_Click(object sender, EventArgs e)
        {
            dataSet.Clear();
            GetNewData();


        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDialog1.Filter = "xls files (*.xls)|*.xls";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = dataSet.Tables["student"].Clone();
                //連線字串
                System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(
                "Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source=" + openFileDialog1.FileName + ";" +
                "Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"");

                connection.Open();

                try
                {
                    string query = "select 學號,班級,姓名 from [Sheet1$]";

                    System.Data.OleDb.OleDbDataAdapter adapter = new System.Data.OleDb.OleDbDataAdapter(query, connection);

                    adapter.Fill(dt);
                    connection.Close();
                    connection.Dispose();
                    //dt.PrimaryKey = new DataColumn[] { dt.Columns["學號"] };
                    string TMPMSG = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (!dataSet.Tables["student"].Rows.Contains(dt.Rows[i]["學號"]))
                        {
                            DataRow newRow = dataSet.Tables["student"].NewRow();
                            newRow["學號"] = dt.Rows[i]["學號"];
                            newRow["班級"] = dt.Rows[i]["班級"];
                            newRow["姓名"] = dt.Rows[i]["姓名"];
                            dataSet.Tables["student"].Rows.Add(newRow);
                            TMPMSG += string.Format("添加 {0},{1},{2}\r\n", newRow["班級"], newRow["學號"], newRow["姓名"]);
                        }
                        else
                        {
                            DataRow oldRow = dataSet.Tables["student"].Rows.Find(dt.Rows[i]["學號"]);
                            oldRow["班級"] = dt.Rows[i]["班級"];
                            oldRow["姓名"] = dt.Rows[i]["姓名"];
                            TMPMSG += string.Format("更新 {0},{1},{2}\r\n", oldRow["班級"], oldRow["學號"], oldRow["姓名"]);
                        }
                    }
                    TMPMSG += string.Format("匯入工作一共處理{0}筆資料", dt.Rows.Count);
                    MessageBox.Show(TMPMSG);
                    dataAdapter3.Update(dataSet, "student");
                    dataSet.AcceptChanges();
                    GetNewData();
                    tssMessage.Text = "學生資料匯入成功!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }



        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "xls files (*.xls)|*.xls";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                DataTable dt = dataSet.Tables["student"].Copy();
                //連線字串
                System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(
                "Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source=" + saveFileDialog1.FileName + ";" +
                "Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=0\"");

                connection.Open();

                //建立工作表
                try
                {
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = connection;

                    DataTable cdt = connection.GetSchema("tables");

                    DataRow[] cdrs = cdt.Select("Table_Name = 'Sheet1'");
                    if (cdrs.Length == 0)
                    {
                        cmd.CommandText = "CREATE TABLE [Sheet1] ([學號] INTEGER,[班級] INTEGER,[姓名] VarChar)";

                        //新增Excel工作表
                        cmd.ExecuteNonQuery();
                    }

                    //增加資料
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["學號"] == null || dt.Rows[i]["班級"] == null || dt.Rows[i]["姓名"] == null)
                        {
                            MessageBox.Show(string.Format("有部分資料錯誤，請檢查 (學號:{0},班級:{1},姓名{2})", dt.Rows[i]["學號"], dt.Rows[i]["班級"], dt.Rows[i]["姓名"]));
                        }
                        else
                        {
                            cmd.CommandText = string.Format("INSERT INTO [Sheet1$] VALUES({0},{1},'{2}')", dt.Rows[i]["學號"], dt.Rows[i]["班級"], dt.Rows[i]["姓名"]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show(string.Format("學生資料成功匯出到 {0}", saveFileDialog1.FileName));
                    tssMessage.Text = "學生資料匯出成功!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                connection.Close();
                connection.Dispose();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //dataSet.Clear();
            GetNewData();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "xls files (*.xls)|*.xls";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                DataTable dt = dataSet.Tables["Total"].Copy();
                //連線字串
                System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(
                "Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source=" + saveFileDialog1.FileName + ";" +
                "Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=0\"");

                connection.Open();

                try
                {
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = connection;

                    DataTable cdt = connection.GetSchema("tables");

                    DataRow[] cdrs = cdt.Select("Table_Name = 'Sheet1'");
                    if (cdrs.Length == 0)
                    {
                        cmd.CommandText = "CREATE TABLE [Sheet1] ([班級] VarChar,[學號] INTEGER,[姓名] VarChar,[目前點數] INTEGER)";

                        //新增Excel工作表
                        cmd.ExecuteNonQuery();
                    }


                    //增加資料
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        cmd.CommandText = string.Format("INSERT INTO [Sheet1$] VALUES('{0}',{1},'{2}',{3})", dt.Rows[i]["CLASS"], dt.Rows[i]["number"], dt.Rows[i]["name"], dt.Rows[i]["POINT"]);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show(string.Format("點數清單成功匯出到 {0}", saveFileDialog1.FileName));
                    tssMessage.Text = "點數清單匯出成功!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                connection.Close();
                connection.Dispose();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDialog1.Filter = "xls files (*.xls)|*.xls";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = dataSet.Tables["pointlog"].Clone();
                //連線字串
                System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(
                "Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source=" + openFileDialog1.FileName + ";" +
                "Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"");

                connection.Open();
                try
                {
                    //dataGridView2.Columns["number"].HeaderText = "學號";
                    //dataGridView2.Columns["stu_class"].HeaderText = "班級";
                    //dataGridView2.Columns["name"].HeaderText = "姓名";
                    //dataGridView2.Columns["date"].HeaderText = "日期";
                    //dataGridView2.Columns["pt_case"].HeaderText = "獎勵事由";
                    //dataGridView2.Columns["point"].HeaderText = "獎勵點數";

                    string query = "select 學號,班級,姓名,日期,獎勵事由,獎勵點數 from [Sheet1$]";

                    System.Data.OleDb.OleDbDataAdapter adapter = new System.Data.OleDb.OleDbDataAdapter(query, connection);

                    adapter.Fill(dt);
                    connection.Close();
                    connection.Dispose();
                    //dt.PrimaryKey = new DataColumn[] { dt.Columns["學號"] };
                    string TMPMSG = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow newRow = dataSet.Tables["pointlog"].NewRow();
                        newRow["number"] = dt.Rows[i]["學號"];
                        newRow["stu_class"] = dt.Rows[i]["班級"];
                        newRow["name"] = dt.Rows[i]["姓名"];
                        newRow["date"] = dt.Rows[i]["日期"];
                        newRow["pt_case"] = dt.Rows[i]["獎勵事由"];
                        newRow["point"] = dt.Rows[i]["獎勵點數"];
                        dataSet.Tables["pointlog"].Rows.Add(newRow);
                        if (i < 30)
                        {
                            TMPMSG += string.Format("添加 {0},{1},{2},{3},{4},{5}\r\n", newRow["stu_class"], newRow["number"], newRow["name"], newRow["date"], newRow["pt_case"], newRow["point"]);
                        }
                    }
                    TMPMSG += string.Format("匯入工作一共處理{0}筆資料", dt.Rows.Count);
                    MessageBox.Show(TMPMSG);
                    dataAdapter2.Update(dataSet, "pointlog");
                    dataSet.AcceptChanges();
                    GetNewData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "xls files (*.xls)|*.xls";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                DataTable dt = dataSet.Tables["pointlog"].Copy();
                //連線字串
                System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(
                "Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source=" + saveFileDialog1.FileName + ";" +
                "Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=0\"");

                connection.Open();

                try
                {
                    //建立工作表
                    //dataGridView2.Columns["number"].HeaderText = "學號";
                    //dataGridView2.Columns["stu_class"].HeaderText = "班級";
                    //dataGridView2.Columns["name"].HeaderText = "姓名";
                    //dataGridView2.Columns["date"].HeaderText = "日期";
                    //dataGridView2.Columns["pt_case"].HeaderText = "獎勵事由";
                    //dataGridView2.Columns["point"].HeaderText = "獎勵點數";
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = connection;

                    DataTable cdt = connection.GetSchema("tables");

                    DataRow[] cdrs = cdt.Select("Table_Name = 'Sheet1'");
                    if (cdrs.Length == 0)
                    {
                        cmd.CommandText = "CREATE TABLE [Sheet1] ([班級] VarChar,[學號] INTEGER,[姓名] VarChar,[日期] VarChar,[獎勵事由] VarChar,[獎勵點數] INTEGER)";

                        //新增Excel工作表
                        cmd.ExecuteNonQuery();
                    }

                    //增加資料
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        cmd.CommandText = string.Format("INSERT INTO [Sheet1$] VALUES('{0}',{1},'{2}','{3}','{4}',{5})", dt.Rows[i]["stu_class"], dt.Rows[i]["number"], dt.Rows[i]["name"], dt.Rows[i]["date"], dt.Rows[i]["pt_case"], dt.Rows[i]["point"]);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show(string.Format("點數清單成功匯出到 {0}", saveFileDialog1.FileName));
                    tssMessage.Text = "點數清單匯出成功!";

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                connection.Close();
                connection.Dispose();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //取出同班級資料
            DataRow[] SSRows = dataSet.Tables["student"].Select(string.Format("班級={0}", dataSet.Tables["class"].Rows[cbxClass.SelectedIndex]["number"]));
            //逐筆加入點數資料
            for (int i = 0; i < SSRows.Length; i++)
            {
                DataRow newRow = dataSet.Tables["pointlog"].NewRow();
                //newRow["id"] = dataSet.Tables["pointlog"].Rows.Count + 1;
                newRow["number"] = SSRows[i]["學號"];
                newRow["stu_class"] = cbxClass.SelectedItem;
                newRow["name"] = SSRows[i]["姓名"];
                newRow["date"] = dtPCDate.Value;
                newRow["point"] = nmUDCPoint.Value;
                newRow["pt_case"] = tbxPTC_case.Text;
                dataSet.Tables["pointlog"].Rows.Add(newRow);
            }
            MessageBox.Show(string.Format("共新增{0}筆資料到點數清單", SSRows.Length));
            dataAdapter2.Update(dataSet, "pointlog");
            dataSet.AcceptChanges();
            tbxPTC_case.Focus();
            tbxPTC_case.SelectAll();
            GetNewData();
            tssMessage.Text = "班級批次點數資料已新增完成!";
        }

        private void button10_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(13))
            {
                if (this.ActiveControl == button10)
                {
                    button10_Click(null, null);
                }
                else
                {
                    this.SelectNextControl(this.ActiveControl, true, true, true, false); //跳下一個元件
                }

            }
        }

    }
}
