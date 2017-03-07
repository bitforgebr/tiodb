using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TioExplorer
{
    public partial class ContainerViewer : Form
    {
        TioClient.Connection m_connection = null;
        TioClient.Container m_container = null;
        string m_server;
        Int16 m_port;
        string m_containerName;

        public ContainerViewer(string server, Int16 port, string containerName)
        {
            m_server = server;
            m_port = port;
            m_containerName = containerName;
            
            InitializeComponent();
        }

        private bool m_updating = false;

        void UpdateData()
        {
            if (m_updating)
            {
                return;
            }

            try
            {
                m_updating = true;

                itemsListView.Items.Clear();

                //
                // let's check if there is a schema for the value
                // 
                string schema = null;
                try
                {
                    schema = m_container.GetProperty("schema");
                }
                catch(Exception){}

                if(!String.IsNullOrEmpty(schema))
                {

                }


                int count = 0;

                m_container.Query(
                    delegate(object key, object value, object metadata)
                    {
                        itemsListView.Items.Add(
                            new ListViewItem(
                                new string[] { Convert.ToString(key), Convert.ToString(value), Convert.ToString(metadata) }));

                        count++;

                        if (count % 1000 == 0)
                        {
                            Application.DoEvents();
                            statusLabel.Text = String.Format("{0} records", count);
                        }
                    });

                statusLabel.Text = String.Format("{0} records", count);
            }
            finally
            {
                m_updating = false;
            }
        }

        private void ContainerViewer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                UpdateData();
            }
        }

        private void ContainerViewer_Load(object sender, EventArgs e)
        {
            Text = m_containerName;
            containerNameTextBox.Text = m_containerName;

            Application.DoEvents();

            try
            {
                m_connection = new TioClient.Connection(m_server, m_port);
                m_container = m_connection.Open(m_containerName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
                Close();
                return;
            }

            UpdateData();
        }

        private void ContainerViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            GC.Collect();
        }

        private void itemsListView_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void itemsListView_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.Control && e.KeyCode == Keys.C )
            {
                String txt = "";
                foreach( ListViewItem i in itemsListView.SelectedItems )
                {
                    String nextTxt = i.Text;
                    if (i.SubItems.Count > 1)
                        nextTxt = nextTxt + ": " + i.SubItems[1].Text;
                    if (txt == "")
                        txt = nextTxt;
                    else
                        txt = txt + "\n" + nextTxt;
                }
                MessageBox.Show(txt);
            }
            else if( e.Control && e.KeyCode == Keys.A )
            {
                foreach (ListViewItem item in itemsListView.Items)
                    item.Selected = true;
            }
        }

        private void itemsListView_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
