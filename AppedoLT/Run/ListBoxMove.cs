using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;


namespace AppedoLT
{
    public partial class ListBoxMove : UserControl
    {
        public ListBoxMove()
        {
            InitializeComponent();
        }
        public void SetItems(DataTable leftData, DataTable rightData)
        {
            if (leftData != null)
            {
                lstLeft.Items.Clear();
                foreach (DataRow dr in leftData.Rows)
                {
                    RadListBoxItem item = new RadListBoxItem();
                    item.Text = dr[1].ToString();
                    item.Value = dr[0];
                    lstLeft.Items.Add(item);
                }
            }
            else
            {
                lstLeft.Items.Clear();
            }
            if (rightData != null)
            {
                lstRight.Items.Clear();
                foreach (DataRow dr in rightData.Rows)
                {
                    RadListBoxItem item = new RadListBoxItem();
                    item.Text = dr[1].ToString();
                    item.Value = dr[0];
                    lstRight.Items.Add(item);
                }
            }
            else
            {
                lstRight.Items.Clear();
            }
        }
        public void ChangeTitle(string leftside, string rightside)
        {
            lblProjectName.Text = leftside;
            lblRight.Text = rightside;
        }
        public void ClearAll()
        {
            try
            {
                lstLeft.Items.Clear();//.DataSource = null;
                lstRight.Items.Clear();//= null;              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] GetItems()
        {
            String[] items = new String[lstLeft.Items.Count];
            int index = 0;
            foreach (RadListBoxItem item in lstLeft.Items)
            {
                items[index++] = item.Value.ToString();
            }
            return items;
        }
        public DataTable GetMappedItems()
        {
            DataTable mappedItems = new DataTable();
            mappedItems.Columns.Add("id");
            mappedItems.Columns.Add("name");
            foreach (RadListBoxItem item in lstLeft.Items)
            {
                DataRow dr= mappedItems.NewRow();
                dr["id"] = item.Value;
                dr["name"] = item.Text;
                mappedItems.Rows.Add(dr);
            }
            return mappedItems;
        }
        private void btnMoveLeftAll_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (RadListBoxItem item in lstRight.Items)
                {
                    RadListBoxItem ss = new RadListBoxItem();
                    ss.Text = item.Text;
                    ss.Value = item.Value;
                    lstLeft.Items.Add(ss);
                }
                lstRight.Items.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnMoveRightAll_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (RadListBoxItem item in lstLeft.Items)
                {
                    RadListBoxItem ss = new RadListBoxItem();
                    ss.Text = item.Text;
                    ss.Value = item.Value;
                    lstRight.Items.Add(ss);
                }
                lstLeft.Items.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnMoveLeftSingle_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstRight.SelectedItem != null)
                {
                    RadListBoxItem receivedItem = (RadListBoxItem)lstRight.SelectedItem;
                    RadListBoxItem ss = new RadListBoxItem();
                    ss.Text = receivedItem.Text;
                    ss.Value = receivedItem.Value;
                    lstLeft.Items.Add(ss);
                    lstRight.Items.Remove(receivedItem);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btnMoveRightSingle_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstLeft.SelectedItem != null)
                {
                    RadListBoxItem receivedItem = (RadListBoxItem)lstLeft.SelectedItem;
                    RadListBoxItem ss = new RadListBoxItem();
                    ss.Text = receivedItem.Text;
                    ss.Value = receivedItem.Value;
                    lstRight.Items.Add(ss);
                    lstLeft.Items.Remove(receivedItem);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ListBoxMove_EnabledChanged(object sender, EventArgs e)
        {
            if (this.Enabled == true)
            {
                lstLeft.SelectionMode = SelectionMode.MultiSimple;
                lstRight.SelectionMode = SelectionMode.MultiSimple;                
                btnMoveLeftSingle.Enabled = true;
                btnMoveLeftAll.Enabled = true;
                btnMoveRightAll.Enabled = true;
                btnMoveRightSingle.Enabled = true;
            }
            else
            {
                lstLeft.SelectionMode = SelectionMode.None;
                lstRight.SelectionMode = SelectionMode.None;                
                btnMoveLeftSingle.Enabled = false;
                btnMoveLeftAll.Enabled = false;
                btnMoveRightAll.Enabled = false;
                btnMoveRightSingle.Enabled = false;
            }
        }     
    }
}
