using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;

namespace EATOUT
{
    public partial class Form1 : Form
    {
        public TabControl myTab = new TabControl();
        public Dictionary<string, Label> statuses = new Dictionary<string, Label>();

        public Form1()
        {
            InitializeComponent();
            myTab.Size = new Size(flowLayoutPanel2.Width, flowLayoutPanel2.Height);
            myTab.Multiline = true;
            myTab.Font = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            this.BackColor = Color.Gainsboro;
            flowLayoutPanel2.Controls.Add(myTab);
            textBox2.Enabled = false;
            button3.Enabled = false;
            textBox1.Focus();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!Char.IsDigit(c) && c != 8 && c != 46)
                e.Handled = true;
            if (Char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == (char)Keys.Enter)
                    button1_Click(sender, e);
                /*else if (e.KeyChar == (char)Keys.Escape)
                    button2_Click(sender, e);
                return;*/
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!Char.IsDigit(c) && c != 8 && c != 46)
                e.Handled = true;
            if (Char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == (char)Keys.Enter)
                    button3_Click(sender, e);
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "ID Заказа")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "ID Заказа";
                textBox2.ForeColor = Color.Silver;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fill_myTab();
            button1.Enabled = false;
            textBox1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var json = doPostCall(textBox1.Text);
            var parsedjsons = parseJson(json);
            var products = getProducts(textBox1.Text);
            var buttons = done_buttons();
            for (int i = 0; i < parsedjsons.Count(); i++)
            {
                var id = parsedjsons[i].OrderID;
                if (myTab.TabPages[i].Text != "ID #" + id) 
                {
                    if (parsedjsons[i].Status == "placed")
                        myTab.TabPages.Add("ID #" + parsedjsons[i].OrderID);

                    Label timeSubmitted = new Label();
                    timeSubmitted.Size = new Size(timeSubmitted.Size.Width + 70, timeSubmitted.Size.Height);
                    timeSubmitted.Location = new Point(0, 6);
                    //timeSubmitted.Font = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
                    timeSubmitted.Text = parsedjsons[i].TimeSubmitted.Remove(0, 11);
                    myTab.TabPages[i].Controls.Add(timeSubmitted);

                    Label timer = new Label();
                    timer.Text = "таймер: " + parsedjsons[i].Timer;
                    //timer.Font = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
                    timer.Size = new Size(timer.Width + 50, timer.Height);
                    timer.Location = new Point(timeSubmitted.Location.X + 200, timeSubmitted.Location.Y);
                    myTab.TabPages[i].Controls.Add(timer);

                    var ps = JsonConvert.DeserializeObject<List<products>>(products);
                    var pss = parsedjsons[i].Contents.ToString().Replace("[", "").Replace("]", "").Split(',');
                    for (int k = 0, m = timeSubmitted.Location.Y + 40; k < pss.Count(); k++, m += 22)
                    {
                        Label p1 = new Label();
                        p1.Location = new Point(timeSubmitted.Location.X, m);
                        p1.Text = getProduct(pss[k].Replace("\"", ""), ps);
                        p1.Size = new Size(p1.Width + 15, p1.Height);
                        //p1.Font = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
                        myTab.TabPages[i].Controls.Add(p1);
                    }

                    myTab.TabPages[i].Controls.Add(statuses[parsedjsons[i].OrderID]);

                    //myTab.TabPages[i].Controls.Add(buttons[i]);
                    textBox2.Enabled = true;
                    button3.Enabled = true;
                }
            }
            for (var i = 0; i < myTab.TabCount; i++)
            {
                myTab.TabPages[i].Font = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
                myTab.TabPages[i].BackColor = Color.Cornsilk;
            }



        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                statuses[textBox2.Text].Text = "Статус: ГОТОВ";
            }
            catch (KeyNotFoundException ex)
            {
                MessageBox.Show("Идентификатор не найден, проверьте правильность введенных данных.");
            }
        }

        List<Button> done_buttons()
        {
            List<Button> l = new List<Button>();
            var json = doPostCall(textBox1.Text);
            var parsedjsons = parseJson(json);
            int i = 1;
            foreach (var o in parsedjsons)
            {
                Button b = new Button();
                b.Name = "done_button" + i.ToString();
                b.Text = "Готов";
                b.Location = new Point(416, 30);
                l.Add(b);
                i += 1;
            }
            return l;
        }

        void status_labels()
        {
            var json = doPostCall(textBox1.Text);
            var parsedjsons = parseJson(json);
            int i = 1;
            foreach (var o in parsedjsons)
            {
                Label lab = new Label();
                lab.Name = "status" + i.ToString();
                lab.Text = "Статус: Готовится";
                lab.Size = new Size(lab.Width + 50, lab.Height);
                lab.Font = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
                lab.Location = new Point(400, 5);
                statuses[o.OrderID] = lab;
                i += 1;
            }
        }

        void fill_myTab()
        {
            status_labels();
            var json = doPostCall(textBox1.Text);
            var parsedjsons = parseJson(json);
            var products = getProducts(textBox1.Text);
            var buttons = done_buttons();
            for (int i = 0; i < parsedjsons.Count(); i++)
            {
                //var id = parsedjsons[i].OrderID;
                //if (myTab.TabPages[0].Text != "ID #" + id) 
                //{
                    if (parsedjsons[i].Status == "placed")
                        myTab.TabPages.Add("ID #" + parsedjsons[i].OrderID);
                    
                    Label timeSubmitted = new Label();
                    timeSubmitted.Size = new Size(timeSubmitted.Size.Width + 70, timeSubmitted.Size.Height);
                    timeSubmitted.Location = new Point(0, 6);
                    //timeSubmitted.Font = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
                    timeSubmitted.Text = parsedjsons[i].TimeSubmitted.Remove(0,11);
                    myTab.TabPages[i].Controls.Add(timeSubmitted);

                    Label timer = new Label();
                    timer.Text = "таймер: " + parsedjsons[i].Timer;
                    //timer.Font = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
                    timer.Size = new Size(timer.Width + 50, timer.Height);
                    timer.Location = new Point(timeSubmitted.Location.X + 200, timeSubmitted.Location.Y);
                    myTab.TabPages[i].Controls.Add(timer);

                    var ps = JsonConvert.DeserializeObject<List<products>>(products);
                    var pss = parsedjsons[i].Contents.ToString().Replace("[", "").Replace("]", "").Split(',');
                    for (int k = 0, m = timeSubmitted.Location.Y + 40; k < pss.Count(); k++, m += 22)
                    {
                        Label p1 = new Label();
                        p1.Location = new Point(timeSubmitted.Location.X, m);
                        p1.Text = getProduct(pss[k].Replace("\"", ""), ps);
                        p1.Size = new Size(p1.Width + 15, p1.Height);
                        //p1.Font = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
                        myTab.TabPages[i].Controls.Add(p1);
                    }

                    myTab.TabPages[i].Controls.Add(statuses[parsedjsons[i].OrderID]);

                    //myTab.TabPages[i].Controls.Add(buttons[i]);
                    textBox2.Enabled = true;
                    button3.Enabled = true;
                //}
            }
            for (var i = 0; i < myTab.TabCount; i++)
            {
                myTab.TabPages[i].Font = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
                myTab.TabPages[i].BackColor = Color.Cornsilk;
            }
        }

        public class json
        {
            public string OrderID { get; set; }
            public string ClientID { get; set; }
            public string TimeSubmitted { get; set; }
            public string Contents { get; set; }
            public string Status { get; set; }
            public string Timer { get; set; }
        }

        public class products
        {
            public string ID { get; set; }
            public string title { get; set; }
        }

        static string getProduct(string ID, List<products> lp)
        {
            return lp[Convert.ToInt32(ID)].title;
        }

        static string doPostCall(string CafeID)
        {
            string URL = "http://test.rightdown.info/db_cafe_fetch_orders.php";
            WebClient webClient = new WebClient();

            NameValueCollection formData = new NameValueCollection();
            formData["CafeID"] = CafeID;

            byte[] responseBytes = webClient.UploadValues(URL, "POST", formData);
            string responsefromserver = Encoding.UTF8.GetString(responseBytes);
            webClient.Dispose();
            return responsefromserver;
        }

        static List<json> parseJson(string json)
            => JsonConvert.DeserializeObject<List<json>>(json);

        static string getProducts(string cafeID)
        {
            string URL = "http://test.rightdown.info/db_cafe_get_products.php";
            WebClient webClient = new WebClient();

            NameValueCollection formData = new NameValueCollection();
            formData["CafeID"] = cafeID;

            byte[] responseBytes = webClient.UploadValues(URL, "POST", formData);
            string responsefromserver = Encoding.UTF8.GetString(responseBytes);
            webClient.Dispose();
            return responsefromserver;
        }
    }
}
