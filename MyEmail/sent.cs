﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
namespace MyEmail
{
    public partial class sent : Form
    {
        public sent()
        {
            InitializeComponent();
        }

        string strCon;
        OleDbConnection sqlCon;
        private void DBConnect()
        {
            strCon = "Provider = Microsoft.Jet.OLEDB.4.0;Data Source =" + Application.StartupPath + @"\MyEmail.mdb";
            sqlCon = new OleDbConnection(strCon);
        }
        private void sent_Load(object sender, EventArgs e)//发件箱加载函数
        {
            DBConnect();
            OleDbDataAdapter da = new OleDbDataAdapter("select 收件人,主题,时间 from send where 发件人='" + login.User + "'", sqlCon);
            DataSet ds = new DataSet();
            da.Fill(ds, "tablename");
            sentdataGridView.DataSource = ds.Tables["tablename"]; 
        }

        private void sentdataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)//双击单元格触发函数
        {
            try
            {
                string body;
                sendmail sent = new sendmail();
                sent.Text = "发件箱信息";
                sent.txtTo.Text = sentdataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
                sent.txtSubject.Text = sentdataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
                DBConnect();
                OleDbDataAdapter da = new OleDbDataAdapter("select 内容 from send where 时间='" + sentdataGridView.Rows[e.RowIndex].Cells[2].Value.ToString() + "'", sqlCon);//("select username as 用户名," + "password as 密码,realname as 真实姓名 from emailuser", sqlCon);
                DataSet ds = new DataSet();
                da.Fill(ds, "tablename");
                body = ds.Tables["tablename"].Rows[0]["内容"].ToString();
                sent.webbody.Document.Write(body);
                sent.Show();
            }
            catch
            {
                
            }
        }
        public static int index;
        private void sentdataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)//记录鼠标点击单元格的行数
        {
            index = e.RowIndex;
        }
        private void deletesent_Click(object sender, EventArgs e)//删除已发送邮件
        {
            try
            {
                if (MessageBox.Show("确定删除邮件吗？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    DBConnect();
                    sqlCon.Open();                   
                    OleDbCommand cmd = new OleDbCommand("delete from send where 发件人='" + login.User + "'and 收件人='" + sentdataGridView.Rows[index].Cells[0].Value.ToString() + "' and 时间='" + sentdataGridView.Rows[index].Cells[2].Value.ToString() + "'", sqlCon);
                    cmd.ExecuteNonQuery();
                    sqlCon.Close();
                    MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    sent_Load(sender, e);//删除后刷新
                }
            }
            catch
            {
                MessageBox.Show("删除失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
    }
}
