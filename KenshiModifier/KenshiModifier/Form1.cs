using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KenshiModifier
{
    public delegate void btn_click(object sender, EventArgs e);
    public partial class Form1 : Form
    {
        Dictionary<string, GroupBox> groups = new Dictionary<string, GroupBox>();
        Dictionary<string, int> value_address = new Dictionary<string, int>();
        string line;
        string current_path;
        public Form1()
        {
            InitializeComponent();
            MakeGUI();
            TSMI_Save.Enabled = false;
        }
        private void MakeGUI()
        {
            string root_capabilities = "capabilities.txt";
            StreamReader sr = new StreamReader(root_capabilities,Encoding.UTF8);
            string line;
            int col = 0, row = 0;
            while((line = sr.ReadLine())!=null)
            {
                string[] tokens = line.Split('-');
                GroupBox groupBox = new GroupBox();
                Label label = new Label();
                TextBox txt = new TextBox();
                Button btn = new Button();

                groupBox.Width = 160; groupBox.Height = 20;
                groupBox.Location = new Point(col * 160, row * 20+30);
                groupBox.Name = "group_" + tokens[0];
                groupBox.Controls.Add(label);
                groupBox.Controls.Add(txt);
                groupBox.Controls.Add(btn);
                this.groups.Add("group_" + tokens[0], groupBox);

                label.Width = 55;label.Height = 18;
                label.Font = new Font("Song", 8);
                label.Location = new Point(0, 1);
                label.Name = "label_" + tokens[0];
                label.Text = tokens[1];
                txt.Width = 45; label.Height = 18;
                txt.Location = new Point(60, 1);
                txt.Name = "txt_" + tokens[0];
                txt.Text = "0.0";
                btn.Width = 50; label.Height = 18;
                btn.Location = new Point(110, 1);
                btn.Name = "btn_"+tokens[0];
                btn.Text = "修改";
                GenBtnFunc(btn);

                groupBox.Enabled = false;
                this.Controls.Add(groupBox);

                col++;
                if(col == 4)
                {
                    col = 0;
                    row++;
                }
            }
            sr.Close();

        }

        private void WriteValue(string valueName,string value)
        {
            int idx = value_address[valueName];
            string token = line.Substring(idx, 4);
            float v = float.Parse(value);
            byte[] buf = BitConverter.GetBytes(v);
            FileStream fs = new FileStream(current_path, FileMode.Open);
            fs.Seek(idx,SeekOrigin.Begin);
            fs.Write(buf, 0, 4);
            fs.Flush();
            fs.Close();
            
            /*
            string new_token = Encoding.ASCII.GetString(buf);
            MessageBox.Show(new_token, "s");
            line.Replace(token, new_token);*/
        }

        private void GenBtnFunc(Button button)
        {
            btn_click temp = delegate (object sender, EventArgs e)
            {
                string ValueName = button.Name.Split('_')[1];
                Control control = this.Controls.Find("txt_" + ValueName, true)[0];
                string Value = control.Text;
                if(ValueName.Contains('#')) ValueName = ValueName.Replace('#', ' ');
                WriteValue(ValueName, Value);
            };
            button.Click += new EventHandler(temp);
        }
        
        private string GetHex(string value)
        {
            float fval = float.Parse(value);
            return ValueHelper.floatToIntString(fval);
        }

        private void TSMI_open_Click(object sender, EventArgs e)
        {
            value_address.Clear();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            
            StreamReader sr = new StreamReader(ofd.FileName,Encoding.ASCII);
            
            current_path = ofd.FileName;
            line = sr.ReadToEnd();
            sr.Close();
            FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            foreach (string key in groups.Keys)
            {
                string name = key.Split('_')[1];
                if (name.Contains('#')) name = name.Replace('#',' ');
                if (line.Contains(name))
                {
                    int len = name.Length;
                    int idx = 0;
                    if (name == "ff" || name == "stealth") idx = line.LastIndexOf(name) + len;
                    else idx = line.IndexOf(name)+len;
                    value_address.Add(name, idx);
                    fs.Seek(idx, SeekOrigin.Begin);
                    string v2 = "";
                    for(int i=0;i<4;i++)
                    {
                        string t = Convert.ToString(br.ReadByte(), 16);
                        if (t.Length == 1) t = "0" + t;
                        v2 += t;
                    }
                    string v = ValueHelper.intStringToFloat(v2).ToString();
                    groups[key].Controls.Find("txt_" + key.Split('_')[1], true)[0].Text = v;
                }
            }
            br.Close();
            fs.Close();
            foreach (GroupBox gb in this.groups.Values)
            {
                gb.Enabled = true;
            }
            this.TSMI_Save.Enabled = true;
        }

        private void TSMI_Save_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter(current_path,false,Encoding.ASCII);
            sw.Write(line);
            sw.Flush();
            sw.Close();
        }
    }
}
