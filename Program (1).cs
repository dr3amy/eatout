using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
//using MySql.Data.MySqlClient;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Runtime.Serialization;


namespace EATOUT
{
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

    class tasks
    {
        static void Main(string[] args)
        {
            //GetRequest("http://test.rightdown.info");
            //PostRequest("http://test.rightdown.info");
            //PostRequest("http://httpbin.org/");
            var json = doPostCall();
            Console.WriteLine(json);
            Console.WriteLine();

            var parsedjsons = parseJson(json);
            Console.WriteLine("jsons parsed: {0}", parsedjsons.Count());
            foreach (var js in parsedjsons)
                Console.WriteLine("<OrderID: {0}; ClientID: {1}; TimeSubmitted: {2}; Contents: {3}; Status: {4}; Timer: {5}>", js.OrderID, js.ClientID, js.TimeSubmitted, js.Contents, js.Status, js.Timer);
            Console.WriteLine();

            var products = getProducts("5");
            Console.WriteLine(products);
            var ps = JsonConvert.DeserializeObject<List<products>>(products);
            foreach (var p in ps)
                Console.WriteLine("<ID: {0}; title: {1}>", p.ID, p.title);
            Console.WriteLine();

            var pss = parsedjsons[0].Contents.ToString().Replace("[", "").Replace("]", "").Split(',');
            foreach (var n in pss) 
                Console.WriteLine(n);

            Console.WriteLine(getProduct("1", ps));

            Console.ReadKey();
        }

        static string getProduct(string ID, List<products> lp)
        {
            return lp[Convert.ToInt32(ID)].title;
        }

        static string doPostCall()
        {
            string URL = "http://test.rightdown.info/db_cafe_fetch_orders.php";
            WebClient webClient = new WebClient();

            NameValueCollection formData = new NameValueCollection();
            formData["CafeID"] = "5";

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

        //static Dictionary<>

        /*  JSON : 
         *  " [{"OrderID":"2","ClientID":"0","TimeSubmitted":"2019-04-17 02:09:00","Contents":"[\"0\",\"2\",\"4\"]","Status":"placed","Timer":"00:02:00"},
         *     {"OrderID":"3","ClientID":"0","TimeSubmitted":"2019-04-17 02:10:00","Contents":"[\"1\",\"3\"]","Status":"placed","Timer":"00:02:00"}] "     
         *
         *  " {"jsons":[{"OrderID":"1","ClientID":"1","TimeSubmitted":"19:17","Contents":"[1, 2, 3]","Status":"Completed","Timer":"2 hrs"},
         *    {"OrderID":"2","ClientID":"2","TimeSubmitted":"19:17","Contents":"[3, 2, 1]","Status":"Completed","Timer":"2 hrs"}]} "
         */
    }

    /*jsonArr arr = new jsonArr();
            arr.jsons = new json[2];
            arr.jsons[0] = new json()
            {
                OrderID = "1",
                ClientID = "1",
                TimeSubmitted = "19:17",
                Contents = "[1, 2, 3]",
                Status = "Completed",
                Timer = "2 hrs"
            };
            arr.jsons[1] = new json()
            {
                OrderID = "2",
                ClientID = "2",
                TimeSubmitted = "19:17",
                Contents = "[3, 2, 1]",
                Status = "Completed",
                Timer = "2 hrs"
            };
            string serialized = JsonConvert.SerializeObject(arr);
            Console.WriteLine(serialized);*/

    /*async static void GetRequest(string url)
{
    using (HttpClient client = new HttpClient())
    {
        using (HttpResponseMessage response = await client.GetAsync(url))
        {
            using (HttpContent content = response.Content)
            {
                string mycontent = await content.ReadAsStringAsync();
                Console.WriteLine(mycontent);
            }

        }
    }
}*/

    /*async static void PostRequest(string url)
    {
        IEnumerable<KeyValuePair<string, string>> queries = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("query1", "STANISLAW"),
            new KeyValuePair<string, string>("query2", "BABY DONT HURT ME"),
            new KeyValuePair<string, string>("query3", "DONT HURT ME"),
            new KeyValuePair<string, string>("query4", "NO MORE")
        };
        HttpContent q = new FormUrlEncodedContent(queries);
        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = await client.PostAsync(url, q))
            {
                using (HttpContent content = response.Content)
                {
                    string mycontent = await content.ReadAsStringAsync();
                    Console.WriteLine(mycontent);
                }

            }
        }
    }*/

    /*public void run()
    {
        // создаем лист для отправки запросов
        ArrayList<NameValuePair> nameValuePairs = new ArrayList<NameValuePair>();
        // один параметр, если нужно два и более просто добоовляем также
        nameValuePairs.add(new BasicNameValuePair("Password", password));
        nameValuePairs.add(new BasicNameValuePair("Email", email));
        //  подключаемся к php запросу и отправляем
        try
        {
            HttpClient httpclient = new DefaultHttpClient();
            HttpPost httppost = new HttpPost("http://test.rightdown.info/db_login.php");
            httppost.setEntity(new UrlEncodedFormEntity(nameValuePairs, "UTF-8"));
            HttpResponse response = httpclient.execute(httppost);
            HttpEntity entity = response.getEntity();
        is = entity.getContent();
            Log.i("pass 1", "connection success ");
        }
        catch (Exception e)
        {
            Log.e("Fail 1", e.toString());
        }

        // получаем ответ от php запроса в формате json
        try
        {
            BufferedReader reader = new BufferedReader(new InputStreamReader(is, "UTF-8"), 8);
            StringBuilder sb = new StringBuilder();
            while ((line = reader.readLine()) != null)
            {
                sb.append(line + "\n");
            }
        is.close();
            result = sb.toString();
            Log.i("pass 2", "connection success" + result);
        }
        catch (Exception e)
        {
            Log.e("Fail 2", e.toString());
        }

        // обрабатываем полученный json
        try
        {
            JSONObject json_data = new JSONObject(result);
            id = (json_data.getString("ID"));
            if (id != null)
            {
                global.id = id;
                global.Name = (json_data.getString("Name"));
                global.Surname = (json_data.getString("Surname"));
                global.isLogged = true;
                Log.e("pass i", global.id);
            }
        }
        catch (Exception e)
        {
            Log.e("Fail 3", e.toString());
        }
    }*/

    /*private static void run2()
    {
        MySqlCommand command = new MySqlCommand(); ;
        string connectionString, commandString;
        //connectionString = "Data source=localhost;UserId=test;Password=informa1;database=test;";
        connectionString = "Server=5.9.25.98; Database=test; Uid=test; Pwd=informa1;";
        MySqlConnection connection = new MySqlConnection(connectionString);
        commandString = "SELECT * FROM `Orders`;";
        command.CommandText = commandString;
        command.Connection = connection;
        MySqlDataReader reader;
        try
        {
            command.Connection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader["OrderID"]);
                /*listBox1.Items.Add(reader["id"]);
                listBox1.Items.Add(reader["name"]);
                listBox1.Items.Add(reader["lastname"]);
            }
            reader.Close();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Error: \r\n{0}", ex.ToString());
        }
        finally
        {
            command.Connection.Close();
        }
    }*/
}

