using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.IO;
using System.Threading;
using System.Security.Policy;
using System.Reflection;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using GroupBox = System.Windows.Forms.GroupBox;
using ComboBox = System.Windows.Forms.ComboBox;
using Button = System.Windows.Forms.Button;
using CheckBox = System.Windows.Forms.CheckBox;
using RadioButton = System.Windows.Forms.RadioButton;
using TextBox = System.Windows.Forms.TextBox;
using Panel = System.Windows.Forms.Panel;
using Image = System.Drawing.Image;
using static System.Windows.Forms.LinkLabel;
using System.Collections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Net.Sockets;
using System.Security.AccessControl;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Web.UI.WebControls;
using static MyServerApp.MyServerApp;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Runtime.Remoting.Messaging;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;
using System.Runtime.InteropServices.ComTypes;

namespace MyServerApp
{
    public partial class MyServerApp : Form
    {
        private HttpListener listener;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private bool isHandlingTextChanged = false;
        int msgBody = 0, count = 0, okcnt = 0, devData = 0, gfile = 0, sfile = 0, rboot = 0, rload = 0, update = 0, attrec = 0, btnClInf = 0, faceFlag = 0, querFlg = 0, btnClientCounter = 0, usrInfoFlag = 0;
        List<Panel> listPanel = new List<Panel>();
        string ImageString;
        // Create a dictionary with string keys and byte array values
        Dictionary<string, byte[]> binaryImageData = new Dictionary<string, byte[]>();
        Dictionary<string, string> values = new Dictionary<string, string>();
        string[] mainSections;
        int index = 0, snflag = 0, succkey = 0;
        byte[] imageBytes = null, fileContentBytes = null;
        string SerialNumber, dftsPinVal;
        string filePath = "", fileName = "", fileContentString = "", cmdStr = "";
        int returnInt = 0;
        public MyServerApp()
        {
            InitializeComponent();
        }
        private bool RuleExists(string name)
        {
            Type policyType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            dynamic firewallPolicy = Activator.CreateInstance(policyType);

            foreach (dynamic rule in firewallPolicy.Rules)
            {
                if (rule.Name == name)
                {
                    return true; // Rule with the same name already exists
                }
            }
            return false; // Rule with the specified name doesn't exist
        }

        private void AddOutboundRule(string name, string description, int portNumber)
        {
            try
            {
                if (RuleExists(name))
                {
                    SetMsgBox("Firewall rule already exists.");
                    return;
                }

                // Create a new rule
                Type type = Type.GetTypeFromProgID("HNetCfg.FwRule");
                dynamic rule = Activator.CreateInstance(type);

                rule.Name = name;
                rule.Description = description;
                rule.Action = 1; // 1 = Allow, 0 = Block
                rule.Direction = 2; // 1 = Inbound, 2 = Outbound
                rule.Protocol = 6; // 6 = TCP
                rule.RemotePorts = portNumber.ToString(); // Set remote port
                rule.Enabled = true;

                // Add the rule to the firewall
                Type policyType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                dynamic firewallPolicy = Activator.CreateInstance(policyType);
                firewallPolicy.Rules.Add(rule);

                SetMsgBox("Outbound firewall rule added successfully.");
            }
            catch (Exception ex)
            {
                SetMsgBox("Failed to add outbound firewall rule: " + ex.Message);
            }
        }
        private void AddInboundRule(string name, string description, int portNumber)
        {
            try
            {
                if (RuleExists(name))
                {
                    //MessageBox.Show("Firewall rule already exists.");
                    SetMsgBox("Firewall rule already exists.");
                    return;
                }
                // Create a new rule
                Type type = Type.GetTypeFromProgID("HNetCfg.FwRule");
                dynamic rule = Activator.CreateInstance(type);

                rule.Name = name;
                rule.Description = description;
                rule.Action = 1;
                rule.Direction = 1; // 1 = Inbound, 2 = Outbound
                rule.Protocol = 6; // 6 = TCP
                rule.LocalPorts = portNumber.ToString();
                rule.Enabled = true;

                // Add the rule to the firewall
                Type policyType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                dynamic firewallPolicy = Activator.CreateInstance(policyType);
                firewallPolicy.Rules.Add(rule);

                //MessageBox.Show("Firewall rule added successfully.");
                SetMsgBox("Firewall rule added successfully.");
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Failed to add firewall rule: " + ex.Message);
                SetMsgBox("Failed to add firewall rule: " + ex.Message);
            }
        }
        private void AddFirewallOutboundRule()
        {
            try
            {
                if (portNo.Text.Length != 0)
                    AddOutboundRule("MyServerApp Outbound Rule", "Allow outbound connections for MyApp", int.Parse(portNo.Text));
            }
            catch (Exception ex)
            {
                SetMsgBox("Exception:" + ex.Message);
            }
        }
        private void AddFirewallInboundRule()
        {
            try
            {
                if (portNo.Text.Length != 0)
                    AddInboundRule("MyServerApp Inbound Rule", "Allow inbound connections for MyApp", int.Parse(portNo.Text.ToString()));
            }
            catch (Exception ex)
            {
                SetMsgBox("Exception:" + ex.Message);
            }
        }
        public class UserData
        {
            public string PIN { get; set; }
            public string Name { get; set; }
            public string Pri { get; set; }
            public string Passwd { get; set; }
            public string Card { get; set; }
            public string Grp { get; set; }
            public string TZ { get; set; }
            public string Verify { get; set; }
            public string ViceCard { get; set; }
            public string StartDatetime { get; set; }
            public string EndDatetime { get; set; }

            public override string ToString()
            {
                return $"PIN: {PIN}, Name: {Name}, Pri: {Pri}, Passwd: {Passwd}, Card: {Card}, Grp: {Grp}, TZ: {TZ}, Verify: {Verify}, ViceCard: {ViceCard}, StartDatetime: {StartDatetime}, EndDatetime: {EndDatetime}";
            }
        }

        public class BioData
        {
            public string Pin { get; set; }
            public string No { get; set; }
            public string Index { get; set; }
            public string Valid { get; set; }
            public string Duress { get; set; }
            public string Type { get; set; }
            public string MajorVer { get; set; }
            public string MinorVer { get; set; }
            public string Format { get; set; }
            public string Tmp { get; set; }

            public override string ToString()
            {
                return $"Pin: {Pin}, No: {No}, Index: {Index}, Valid: {Valid}, Duress: {Duress}, Type: {Type}, MajorVer: {MajorVer}, MinorVer: {MinorVer}, Format: {Format}, Tmp: {Tmp}";
            }
        }

        public class UserPic
        {
            public string Pin { get; set; }
            public string FileName { get; set; }
            public string Size { get; set; }
            public string Content { get; set; }

            public override string ToString()
            {
                return $"Pin: {Pin}, FileName: {FileName}, Size: {Size}, Content: {Content}";
            }
        }

        public class BioPhoto
        {
            public string PIN { get; set; }
            public string FileName { get; set; }
            public string Type { get; set; }
            public string Size { get; set; }
            public string Content { get; set; }

            public override string ToString()
            {
                return $"PIN: {PIN}, FileName: {FileName}, Type: {Type}, Size: {Size}, Content: {Content}";
            }
        }

        public class UserManager
        {
            private const string SP = " ";
            private const string HT = "\t";

            public static List<UserData> ParseUsers(string data)
            {
                List<UserData> users = new List<UserData>();
                string[] userSections = data.Split(new string[] { "USER" + SP }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var section in userSections)
                {
                    if (section.Trim().StartsWith("PIN"))
                    {
                        var user = new UserData();
                        ParseSection(section, user, typeof(UserData));
                        users.Add(user);
                    }
                }
                return users;
            }

            public static List<BioData> ParseBioData(string data)
            {
                List<BioData> bioDatas = new List<BioData>();
                string[] bioDataSections = data.Split(new string[] { "BIODATA" + SP }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var section in bioDataSections)
                {
                    if (section.Trim().StartsWith("Pin"))
                    {
                        var bioData = new BioData();
                        ParseSection(section, bioData, typeof(BioData));
                        bioDatas.Add(bioData);
                    }
                }
                return bioDatas;
            }

            public static List<UserPic> ParseUserPics(string data)
            {
                List<UserPic> userPics = new List<UserPic>();
                string[] userPicSections = data.Split(new string[] { "PIC" + SP }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var section in userPicSections)
                {
                    if (section.Trim().StartsWith("PIN"))
                    {
                        var userPic = new UserPic();
                        ParseSection(section, userPic, typeof(UserPic));
                        userPics.Add(userPic);
                    }
                }
                return userPics;
            }

            public static List<BioPhoto> ParseBioPhotos(string data)
            {
                List<BioPhoto> bioPhotos = new List<BioPhoto>();
                string[] bioPhotoSections = data.Split(new string[] { "BIOPHOTO" + SP }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var section in bioPhotoSections)
                {
                    if (section.Trim().StartsWith("PIN"))
                    {
                        var bioPhoto = new BioPhoto();
                        ParseSection(section, bioPhoto, typeof(BioPhoto));
                        bioPhotos.Add(bioPhoto);
                    }
                }
                return bioPhotos;
            }

            private static void ParseSection(string section, object obj, Type type)
            {
                string[] parts = section.Split(new[] { '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
                foreach (var prop in type.GetProperties())
                {
                    properties[prop.Name] = prop;
                }

                foreach (var part in parts)
                {
                    if (part.Contains("="))
                    {
                        string[] keyValue = part.Split(new[] { '=' }, 2);
                        if (keyValue.Length == 2)
                        {
                            string key = keyValue[0].Trim();
                            string value = keyValue[1];
                            if (value != "")
                            {
                                if (properties.TryGetValue(key, out var property))
                                {
                                    if (property.CanWrite)
                                    {
                                        property.SetValue(obj, value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void MyServerApp_Load(object sender, EventArgs e)
        {
            // Get the host name of the local machine
            string hostName = Dns.GetHostName();

            // Get all IP addresses associated with the local machine
            IPAddress[] localIPs = Dns.GetHostAddresses(hostName);

            // Filter IPv4 addresses
            IPAddress ipv4Address = localIPs.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            if (ipv4Address != null)
            {
                serverIP.Text = ipv4Address.ToString();
            }

            for (int i = 0; i < 60; i++)
            {
                if (i < 10)
                {
                    if (i < 24)
                    {
                        this.comboBox1.Items.Add("0" + i.ToString());
                    }
                    this.comboBox2.Items.Add("0" + i.ToString());
                }
                else
                {
                    if (i < 24)
                    {
                        this.comboBox1.Items.Add(i.ToString());
                    }
                    this.comboBox2.Items.Add(i.ToString());
                }
            }
            if (selectUserPIN.Items.Count == 0)
            {
                selectUserPIN.Text = "No User Data";
                getUsrData.Enabled = false;
            }
            this.WindowState = FormWindowState.Maximized;
            UserManager userManager = new UserManager();
            usrPin.Enabled = false;
            selectUserPIN.Enabled = false;
            //enrollPalm.Enabled = false;
            enrollCard.Enabled = false;
            LabelSNSelect.Visible = false;  
            dftsPinVal = SNComboBox.Text;
        }
        private void UpdateUserDetails(UserData user)
        {
            SetTextBoxValue(usrNamBox, user.Name);
            SetTextBoxValue(privilegeBox, user.Pri);
            SetTextBoxValue(passwd, user.Passwd);
            SetTextBoxValue(crdBox, user.Card);
            SetTextBoxValue(grpBox, user.Grp);
            SetTextBoxValue(tzBox, user.TZ);
            SetTextBoxValue(verifyBox, user.Verify);
            SetTextBoxValue(vcBox, user.ViceCard);
            SetTextBoxValue(StrtDatTimBox, user.StartDatetime);
            SetTextBoxValue(EndDatTimeBox, user.EndDatetime);
        }
        private void fillUserSpecificDetails(int index)
        {
            int countIndex = 0;
            for (int i = index; i < mainSections.Length; i++)
            {
                if(mainSections[i].IndexOf("PIN=" + selectUserPIN.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (mainSections[i].Contains("TZ=") && mainSections[i].Contains("PIN=" + selectUserPIN.Text) && (countIndex == 0))
                    {
                        List<UserData> users = UserManager.ParseUsers(mainSections[i]);
                        foreach (var user in users)
                        {
                            SetComboBoxValue(selectUserPIN, user.PIN);
                            UpdateUserDetails(user);
                        }
                    }
                    else if (mainSections[i].StartsWith("PIC") && mainSections[i].Contains("Content") && mainSections[i].Contains("PIN=" + selectUserPIN.Text) && (countIndex == 0))
                    {
                        List<UserPic> userPics = UserManager.ParseUserPics(mainSections[i]);
                        foreach (var userPic in userPics)
                        {
                            SetTextBoxValue(fnBox, userPic.FileName);
                            SetTextBoxValue(szBox, userPic.Size);
                        }
                    }
                    else if (mainSections[i].Contains("Type=") && mainSections[i].Contains("Content") && mainSections[i].Contains("PIN=" + selectUserPIN.Text) && (countIndex == 0))
                    {
                        List<BioPhoto> bioPhotos = UserManager.ParseBioPhotos(mainSections[i]);
                        foreach (var bioPhoto in bioPhotos)
                        {
                            SetTextBoxValue(fnBox1, bioPhoto.FileName);
                            SetTextBoxValue(sz1, bioPhoto.Size);
                            SetTextBoxValue(type1, bioPhoto.Type);
                        }
                    }
                    else if (mainSections[i].Contains("Tmp") && mainSections[i].Contains("Pin=" + selectUserPIN.Text))
                    {
                        List<BioData> bioDatas = UserManager.ParseBioData(mainSections[i]);
                        foreach (var bioData in bioDatas)
                        {
                            SetTextBoxValue(noBox, bioData.No);
                            SetTextBoxValue(indexBox, bioData.Index);
                            SetTextBoxValue(validBox, bioData.Valid);
                            SetTextBoxValue(duressBox, bioData.Duress);
                            SetTextBoxValue(typBox, bioData.Type);
                            SetTextBoxValue(MajVerBox, bioData.MajorVer);
                            SetTextBoxValue(MinVerBox, bioData.MinorVer);
                            SetTextBoxValue(frmtBox, bioData.Format);
                        }
                        countIndex++;
                        if (countIndex == 6)
                        {
                            countIndex = 0;
                        }
                    }
                }
            }
        }
        private void populateUserData()
        {
            string data = LogText.Text.ToString();
            string[] allSeparators = { "USER", "BIODATA", "BIOPHOTO", "USERPIC" };
            // Create a list to store the separators that are present in the data
            List<string> presentSeparators = new List<string>();
            // Check each separator for its presence in the data
            foreach (string separator in allSeparators)
            {
                if (data.Contains(separator))
                {
                    presentSeparators.Add(separator);
                }
            }
            // Convert the list of present separators to an array
            string[] separators = presentSeparators.ToArray();

            mainSections = data.Split(separators, StringSplitOptions.None);

            for (int i = 1; i < mainSections.Length; i++)
            {
                if (mainSections[i].Contains("TZ"))
                {
                    List<UserData> users = UserManager.ParseUsers(mainSections[i]);
                    foreach (var user in users)
                    {
                        SetComboBoxValue(selectUserPIN, user.PIN);
                    }
                    if (selectUserPIN.Items.Count > 0)
                    {
                        if (selectUserPIN.InvokeRequired)
                        {
                            selectUserPIN.Invoke(new Action(() =>
                            {
                                selectUserPIN.Enabled = true;
                                selectUserPIN.Text = "Select User";
                            }));
                        }
                        else
                        {
                            selectUserPIN.Enabled = true;
                            selectUserPIN.Text = "Select User";
                        }
                    }
                }
            }
        }
        private void SetControlsReadOnly(System.Windows.Forms.GroupBox groupBox, bool readOnly)
        {
            foreach (Control control in groupBox.Controls)
            {
                // Set Enabled property of controls to false to make them read-only
                control.Enabled = !readOnly;

                // If the control is a container control (i.e., another GroupBox), recursively set its controls to read-only
                if (control is System.Windows.Forms.GroupBox nestedGroupBox)
                {
                    SetControlsReadOnly(nestedGroupBox, readOnly);
                }
            }
        }
        public string ExtractSN(string input)
        {
            // Find the index of the "SN=" substring
            int startIndex = input.IndexOf("SN=");
            string snValue = "";
            // Check if "SN=" exists in the string
            if (startIndex != -1)
            {
                // Extract the substring starting from the index of "SN="
                string snSubstring = input.Substring(startIndex + 3); // Adding 3 to skip "SN="

                // Find the index of the next "&" character to determine the end of the SN value
                int endIndex = snSubstring.IndexOf("&");

                // If "&" is found, extract the SN value, otherwise, use the entire substring
                snValue = (endIndex != -1) ? snSubstring.Substring(0, endIndex) : snSubstring;

                //SetMsgBox("\nSN value: " + snValue);
            }
            else
            {
                SetMsgBox("\nSN parameter not found.\n");
            }
            return snValue;
        }
        private void SetCheckBox(CheckBox chkBoxName, bool msg)
        {
            if (chkBoxName.InvokeRequired)
            {
                chkBoxName.Invoke((MethodInvoker)(() => chkBoxName.Checked = msg));
            }
            else
            {
                chkBoxName.Checked = msg;
            }
        }
        private void SetMsgBox(string msg)
        {
            if (msgBox.InvokeRequired)
            {
                msgBox.Invoke((MethodInvoker)(() => msgBox.Text += "\n\r" + msg));
            }
            else
            {
                msgBox.Text += "\n\r" + msg;
            }
        }
        private void SetCmdHstBox(string msg)
        {
            if (cmdhstBox.InvokeRequired)
            {
                cmdhstBox.Invoke((MethodInvoker)(() => cmdhstBox.Text += "\r\n" + msg));
            }
            else
            {
                cmdhstBox.Text += "\r\n" + msg;
            }
            cmdhstBox.AppendText(Environment.NewLine);
        }
        private void SetLogTextBox(string msg)
        {
            if (msgBox.InvokeRequired)
            {
                LogText.Invoke((MethodInvoker)(() => LogText.Text += msg + "\n"));
            }
            else
            {
                LogText.Text += msg + "\n";
            }
        }
        private void SetResponseBox(string msg)
        {
            if (msgBox.InvokeRequired)
            {
                responseBox.Invoke((MethodInvoker)(() => responseBox.Text += msg + "\r\n"));
            }
            else
            {
                responseBox.Text += msg + "\r\n";
            }
        }
        private void modifyLabel(ComboBox comboBox)
        {
            if (comboBox.Name == "SNComboBox")
            {
                if (LabelSNSelect.InvokeRequired)
                {
                    LabelSNSelect.Invoke(new MethodInvoker(() => LabelSNSelect.Visible = true));
                }
                else
                {
                    LabelSNSelect.Visible = true;
                }
                LabelSNSelect.ForeColor = Color.Green;
            }
        }
        private void SetComboBoxValue(ComboBox comboBox, string msg)
        {
            try
            {
                if (!comboBox.Items.Contains(msg))
                {
                    if (comboBox.InvokeRequired)
                    {
                        comboBox.Invoke((MethodInvoker)(() => comboBox.Items.Add(msg)));
                    }
                    else
                    {
                        comboBox.Items.Add(msg);
                    }
                    modifyLabel(comboBox);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                SetMsgBox(ex.Message);
            }
        }
        private void startBtn_Click(object sender, EventArgs e)
        {
            AddFirewallOutboundRule();
            AddFirewallInboundRule();
            SetControlsReadOnly(setConfig, true);
            startBtn.Enabled = false;
            stopBtn.Enabled = true;
            if (portNo.Text.ToString() != "")
            {
                try
                {
                    string[] prefixes = { "http://" + serverIP.Text + ":" + portNo.Text + "/" };
                    InitServer(prefixes);
                }
                catch (Exception ex)
                {
                    string s = "ERROR: " + ex.Message;
                    SetMsgBox(s);
                }
            }
            else
            {
                startBtn.Enabled = true;
                stopBtn.Enabled = false;
                msgBox.Text += "\nEnter port number first.";
            }

        }
        public void InitServer(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                msgBox.Text += "\nWindows XP SP2 or Server 2003 or higher version is required to use the HttpListener class.";
                return;
            }
            // Create a listener.
            listener = new HttpListener();
            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }
            StartListening();
        }
        private void transFlag_Enter(object sender, EventArgs e)
        {

        }
        private void SetTextBoxValue(TextBox textBox, string value)
        {
            if (textBox.InvokeRequired)
            {
                textBox.Invoke(new MethodInvoker(delegate
                {
                    textBox.Text = value;
                }));
            }
            else
            {
                textBox.Text = value;
            }
        }
        private void SetRadioValue(RadioButton radioButton, bool enable)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    radioButton.Checked = enable;
                }));
            }
            else
            {
                radioButton.Checked = enable;
            }
        }
        private void StartListening()
        {
            listener.Start();
            if (msgBox != null && !string.IsNullOrEmpty(msgBox.Text))
            {
                SetMsgBox("\nListening...");
            }
            else
            {
                SetMsgBox("Listening...");
            }

            // Start a new thread to handle incoming requests
            System.Threading.ThreadPool.QueueUserWorkItem((_) =>
            {
                while (listener.IsListening)
                {
                    try
                    {
                        // Note: The GetContext method blocks while waiting for a request.
                        HttpListenerContext context = listener.GetContext();
                        if (context != null)
                        {
                            HandleRequest(context);
                        }
                    }
                    catch (Exception ex)
                    {
                        string s = "\nError: " + ex.Message;
                        SetMsgBox(s);
                    }
                }
            });
        }
        int updFacFlg = 0;

        private byte[] ImageToByteArray(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }
        void EnterUpdateValue(int caseValue)
        {
            switch (caseValue)
            {
                case 1:
                    usrNamBox.Enabled = true; usrNamBox.ReadOnly = false;
                    privilegeBox.Enabled = true; privilegeBox.ReadOnly = false;
                    passwd.Enabled = true;passwd.ReadOnly = false;
                    crdBox.Enabled = true;crdBox.ReadOnly = false;
                    grpBox.Enabled = true; grpBox.ReadOnly = false;
                    tzBox.Enabled = true;tzBox.ReadOnly = false;
                    verifyBox.Enabled = true;verifyBox.ReadOnly = false;
                    vcBox.Enabled = true;vcBox.ReadOnly = false;
                    break;
                case 2:
                    usrNamBox.Enabled = false; usrNamBox.ReadOnly = true;
                    privilegeBox.Enabled = false; privilegeBox.ReadOnly = true;
                    passwd.Enabled = false; passwd.ReadOnly = true;
                    crdBox.Enabled = false; crdBox.ReadOnly = true;
                    grpBox.Enabled = false; grpBox.ReadOnly = true;
                    tzBox.Enabled = false; tzBox.ReadOnly = true;
                    verifyBox.Enabled = false; verifyBox.ReadOnly = true;
                    vcBox.Enabled = false; vcBox.ReadOnly = true;
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        }
        private void SetButtonEnabled(Button button, bool enabled)
        {
            // Check if the call needs to be marshaled to the UI thread
            if (button.InvokeRequired)
            {
                button.Invoke(new Action(() => button.Enabled = enabled));
            }
            else
            {
                button.Enabled = enabled;
            }
        }
        public string getCommand()
        {
            int index;
            succkey = 0;
            string value;//,s = "";
            verify1to1.Checked = false;
            
            if ((snflag == 1 && cmdID.Text != "") || loadInfo.Checked)
            {
                cmdStr = ":INFO";
                succkey = 1;
                SetCheckBox(loadInfo, false);
            }
            else
            {
                GetComboBoxValueAndIndex(SNComboBox, out value, out index);
                if ((index != -1 && value != null) || (value == SerialNumber))
                {
                    if (rboot == 1 && reboot.Checked == true)
                    {
                        cmdStr = ":REBOOT";
                        succkey = 2;
                    }
                    if (reload.Checked == true)
                    {
                        cmdStr = ":CHECK"; succkey = 3;
                    }
                    if (rmtEnrollFP.Checked && !sndRmtEnrollmentCmd.Enabled)
                    {
                        cmdStr = string.Format(":ENROLL_FP PIN={0}\tFID={1}\tRETRY={2}\tOVERWRITE={3}", rmtPinBox.Text, fidBox.Text, retryCnt.Text, overWrite.Text);
                        SetRadioValue(rmtEnrollFP, false);
                        sndRmtEnrollmentCmd.Enabled = true; succkey = 4;
                    }
                    if (enrollFace.Checked && !sndRmtEnrollmentCmd.Enabled)
                    {
                        cmdStr = string.Format(":ENROLL_FP PIN={0}\tFID={1}\tRETRY={2}\tOVERWRITE={3}", rmtPinBox.Text, fidBox.Text, retryCnt.Text, overWrite.Text);
                        SetRadioValue(enrollFace, false);
                        sndRmtEnrollmentCmd.Enabled = true;succkey = 5;
                    }
                    if (enrollPalm.Checked && !sndRmtEnrollmentCmd.Enabled)
                    {
                        cmdStr = string.Format(":ENROLL_BIO TYPE={0}\tPIN={1}\tCardNo={2}\tRETRY={3}\tOVERWRITE={4}", bioType.Text, rmtPinBox.Text, enterCard.Text, retryCnt.Text, overWrite.Text);
                        SetRadioValue(enrollPalm, false);
                        sndRmtEnrollmentCmd.Enabled = true;succkey = 6;
                    }
                    if (enrollCard.Checked && !sndRmtEnrollmentCmd.Enabled)
                    {
                        cmdStr = string.Format(":ENROLL_MF PIN={0}\tRETRY={1}", rmtPinBox.Text, retryCnt.Text);
                        SetRadioValue(enrollCard, false);
                        sndRmtEnrollmentCmd.Enabled = true;succkey=7;
                    }
                    if (!updUserInfo.Enabled && updUsrPic.Checked)
                    {
                        cmdStr = string.Format(":DATA UPDATE USERPIC PIN={0}\tSize={1}\tContent={2}", selectUserPIN.Text, ImageString.Length, ImageString);
                        SetRadioValue(updUsrPic, false);
                        updUserInfo.Enabled = true;succkey = 8;
                    }
                    if (!updUserInfo.Enabled && updUsrInf.Checked)
                    {
                        cmdStr = String.Format(":DATA UPDATE USERINFO PIN={0}\tName={1}\tPri={2}\tPasswd={3}\tCard={4}\tGrp={5}\tTZ={6}\tVerify={7}\tViceCard={8}", selectUserPIN.Text, usrNamBox.Text, privilegeBox.Text, passwd.Text, crdBox.Text, grpBox.Text, tzBox.Text, verifyBox.Text, vcBox.Text);
                        updUserInfo.Enabled = true;
                        SetRadioValue(updUsrInf, false);
                        EnterUpdateValue(2);succkey = 9;
                    }
                    if (!updUserInfo.Enabled && updFace.Checked)
                    {
                        if (updFacFlg == 0)
                        {
                            cmdStr = string.Format(":DATA UPDATE BIOPHOTO PIN={0}\tType={1}\tFormat={2}\tUrl={3}", "1", "9", "1", "C:/Users/himanshu/Pictures/Camera/sam.jpg");//"/ mnt/mtdblock/data/biophoto/face"
                            updFacFlg = 1;succkey = 10;
                        }
                    }
                    if (!btnClientInfo.Enabled)
                    {
                        if ((selectUserPIN.Enabled || usrPin.Enabled) && btnClInf == 1)
                        {
                            cmdStr = ":DATA QUERY USERINFO PIN=" + usrPin.Text;
                            usrInfoFlag = 1; btnClInf = 0;succkey = 11;
                        }
                        else
                        {
                            if (btnClInf == 0 && btnClientCounter == 1)
                            {
                                cmdStr = ":DATA QUERY USERINFO";
                                usrInfoFlag = 1;succkey = 12;
                            }
                        }
                        if (selectUserPIN.InvokeRequired)
                        {
                            selectUserPIN.Invoke(new Action(() => selectUserPIN.Items.Clear()));
                        }
                        else
                        {
                            selectUserPIN.Items.Clear();
                        }
                    }
                    if(!getFile.Enabled)
                    {
                        cmdStr = String.Format(":GetFile {0}/{1}", filePat.Text, fName.Text);succkey = 13;
                        //SetButtonEnabled(getFile,true);
                    }
                    if (!sendFile.Enabled && filePath != "")
                    {
                        if(fileName.Contains(".tgz"))
                        {
                            cmdStr = String.Format(":PutFile {0}\t{1}", filePath, "/mnt/mtdblock/him/");
                        }
                        else
                        {
                            cmdStr = String.Format(":PutFile {0}\t{1}", filePath, "/mnt/mtdblock/him/" + fileName);
                        }
                        SetButtonEnabled(sendFile, true);
                        succkey = 14;
                    }
                }
            }
            if (cmdStr.Length > 0) {
                Random rnd = new Random();
                int num = rnd.Next();
                SetTextBoxValue(cmdID, num.ToString());
                cmdStr = "C:" + cmdID.Text + cmdStr;
            }
            return cmdStr;
        }
        private void setResultBoxValue(int succkey)
        {
            SetTextBoxValue(resultBox, "");
            switch (succkey)
            {
                case 0: break;
                case 1:
                    SetTextBoxValue(resultBox, "Device Info Fetched Successfully\n");
                    break;
                case 2:
                    SetTextBoxValue(resultBox, "Reboot Successful\n");
                    break;
                case 3:
                    SetTextBoxValue(resultBox, "Reload Successful\n");
                    break;
                case 4:
                    if(verify1to1.Checked)
                    {
                        SetTextBoxValue(resultBox, "Remotely Verification Command Sent\n");
                    }
                    else
                    {
                        if (returnInt == 4 || returnInt == 5)
                        {
                            SetTextBoxValue(resultBox, "Remotely Enroll Fingerprint Failed\n");
                        }
                        else
                        {
                            SetTextBoxValue(resultBox, "Remotely Enroll Fingerprint Successful\n");
                        }
                    }
                    break;
                case 5:
                    if(returnInt == 0) 
                        SetTextBoxValue(resultBox, "Remotely Enroll Face Successful\n");
                    else
                        SetTextBoxValue(resultBox, "Remotely Enroll Face Failed |OR| Face already exist.\n");
                    break;
                case 6:
                    SetTextBoxValue(resultBox, "Remotely Enroll Palm Successful\n");
                    break;
                case 7:
                    SetTextBoxValue(resultBox, "Remotely Enroll Card Successful\n");
                    break;
                case 8:
                    SetTextBoxValue(resultBox, "Update User Pic Successful\n");
                    break;
                case 9:
                    SetTextBoxValue(resultBox, "Update User Info Successful\n");
                    break;
                case 10:
                    SetTextBoxValue(resultBox, "BioPhoto Update Successful\n");
                    break;
                case 11:
                    SetTextBoxValue(resultBox, "Query User " + usrPin.Text + " Successful\n");
                    break;
                case 12:
                    SetTextBoxValue(resultBox, "Query All User Info Successful\n");
                    break;
                case 13:
                    SetTextBoxValue(resultBox, fName.Text + " received successfully\n");
                    break;
                case 14:
                    SetTextBoxValue(resultBox, fileName + " sent successfully\n");
                    break;
            }
        }
        static int FindByteArray(byte[] haystack, byte[] needle)
        {
            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < needle.Length; j++)
                {
                    if (haystack[i + j] != needle[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }

        // Method to combine multiple byte arrays into one
        static byte[] CombineArrays(params byte[][] arrays)
        {
            int totalLength = 0;
            foreach (byte[] array in arrays)
            {
                totalLength += array.Length;
            }

            byte[] result = new byte[totalLength];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }

            return result;
        }

        private void GetComboBoxValueAndIndex(ComboBox comboBox, out string selectedValue, out int selectedIndex)
        {
            selectedValue = null;
            selectedIndex = -1;

            try
            {
                if (comboBox.InvokeRequired)
                {
                    string tempValue = null;
                    int tempIndex = -1;

                    comboBox.Invoke(new MethodInvoker(() =>
                    {
                        tempValue = comboBox.Text?.ToString();
                        tempIndex = comboBox.SelectedIndex;
                    }));

                    selectedValue = tempValue;
                    selectedIndex = tempIndex;
                }
                else
                {
                    selectedValue = comboBox.Text?.ToString();
                    selectedIndex = comboBox.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                SetMsgBox(ex.Message);
            }
        }

        private void SetConfBoxValue(string input, string value)
        {
            switch(input)
            {
                case "~DeviceName": SetTextBoxValue(dvcNameBox,value);break;
                case "MAC": SetTextBoxValue(macBox, value);break;
                case "UserCount": SetTextBoxValue(usrCntBox, value); break;
                case "FaceCount": SetTextBoxValue(facCntBox, value);break;
                case "FvCount": SetTextBoxValue(fvCntBox, value);break;
                case "FPCount": SetTextBoxValue(fpCountBox, value); break;
                case "PvCount": SetTextBoxValue(pvCntBox, value); break;
                case "IPAddress": SetTextBoxValue(ipaddrBox, value); break;
                case "~Platform": SetTextBoxValue(platfrmBox, value);break;
                case "PushVersion": SetTextBoxValue(pushVerBox, value);break;
            }
        }
        private void SetConfParameter(String input)
        {
            // Split the string into key-value pairs
            string[] pairs = input.Split(',');

            // Extract keys and values
            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split('=');

                if (keyValue.Length == 2)
                {
                    string key = keyValue[0];
                    string value = keyValue[1];
                    values[key] = value;
                    SetConfBoxValue(key, value);
                }
                else
                {
                    // Handle cases where there is no value or key
                    string key = keyValue[0];
                    values[key] = string.Empty;
                }
            }
        }
        private void HandleRequest(HttpListenerContext context)
        {
            string s = "", st=""; 
            HttpListenerRequest request = context.Request;
            s = "\r\n\r\nReceived request from: " + request.RemoteEndPoint.Address + " "+ "\n";
            st = request.HttpMethod;
            s = s + st + request.RawUrl + "  " + "HTTP/" + request.ProtocolVersion + " "+ "\n";
            string contentType = request.ContentType;
            if (contentType != null)
            {
                s += contentType + "\n";
            }
            foreach (string key in request.Headers.AllKeys)
            {
                s += "\r\n"+ key + ": " + request.Headers[key]+"\n";
            }
            SetMsgBox(s);

            if (( updFacFlg==0 && request.Url.AbsolutePath.Contains("/Camera/") == false)) 
            {
                SerialNumber = ExtractSN(request.RawUrl);

                if (SerialNumber != null)
                {
                    SetComboBoxValue(SNComboBox, SerialNumber);
                }
            }

            int index;
            string serialNumberSelected;
            GetComboBoxValueAndIndex(SNComboBox, out serialNumberSelected, out index);

            okcnt = 0; devData = 0;

            try
            {
                if ((index != -1 && serialNumberSelected != null) && (serialNumberSelected == SerialNumber))
                {
                    if (context.Request.HasEntityBody)
                    {
                        SetMsgBox("\n");
                        devData = 1;
                        List<byte> binaryDataBytes = new List<byte>();
                        string pin = null;
                        string serialNumber = null;
                        string size = null;
                        string line;
                        byte[] LF = Encoding.UTF8.GetBytes("\n");
                        byte[] NUL = new byte[] { 0 };
                        //int done = 0;
                        byte[] newImgData = null;
                        using (Stream body = context.Request.InputStream)
                        {
                            if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/iclock/fdata")
                            {
                                faceFlag = 1;

                                // Get the length of the request body
                                long length = context.Request.ContentLength64;

                                // Create a buffer to hold the request body
                                byte[] buffer = new byte[length];

                                // Read the request body into the buffer
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    context.Request.InputStream.CopyTo(memoryStream);
                                    buffer = memoryStream.ToArray();
                                }
                                binaryDataBytes.AddRange(buffer);
                                string headers = string.Empty;
                                int headerLength = 0;
                                for (int i = 0; i < binaryDataBytes.Count; i++)
                                {
                                    // Check for the end of the headers (NUL character)
                                    if (binaryDataBytes[i] == 0)
                                    {
                                        headerLength = i + 1;
                                        headers = System.Text.Encoding.UTF8.GetString(binaryDataBytes.ToArray(), 0, i);
                                        break;
                                    }
                                }
                                // Extract binary data after the headers
                                byte[] binaryData = new byte[binaryDataBytes.Count - headerLength];
                                string input = "CMD=uploadphoto";

                                // Convert the string to a byte array using UTF-8 encoding
                                byte[] byteArray = Encoding.UTF8.GetBytes(input);
                                int startIndex = FindByteArray(binaryDataBytes.ToArray(), byteArray);
                                if (startIndex != -1)
                                {
                                    // Calculate the starting index of the binary data
                                    int binaryDataStartIndex = startIndex + byteArray.Length + 1;

                                    // Create a new array to store the binary data
                                    int binaryDataLength = binaryDataBytes.ToArray().Length - binaryDataStartIndex;
                                    newImgData = new byte[binaryDataLength];
                                    Array.Copy(binaryDataBytes.ToArray(), binaryDataStartIndex, newImgData, 0, binaryDataLength);
                                }
                                if (newImgData != null && newImgData.ToArray().Length > 0)
                                {
                                    using (MemoryStream ms = new MemoryStream(newImgData))
                                    {
                                        ms.Position = 0; // Ensure stream position is at the beginning
                                        picBox.Image = Image.FromStream(ms);
                                        picBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                    }
                                }
                            }
                            else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/iclock/devicecmd" && gfile == 1)
                            {
                                // Get the length of the request body
                                long length = context.Request.ContentLength64;

                                // Create a buffer to hold the request body
                                byte[] buffer = new byte[length];

                                // Read the request body into the buffer
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    context.Request.InputStream.CopyTo(memoryStream);
                                    buffer = memoryStream.ToArray();
                                }
                                binaryDataBytes.AddRange(buffer);

                                string headers = string.Empty;
                                string headers1 = string.Empty;
                                int headerLength = 0;
                                string contentKeyword = "Content=";
                                int contentStartIndex = -1;

                                for (int i = 0; i < binaryDataBytes.Count; i++)
                                {
                                    headers1 = System.Text.Encoding.UTF8.GetString(binaryDataBytes.ToArray(), 0, headerLength);
                                    // Check for the start of the Content field
                                    if (i + contentKeyword.Length <= binaryDataBytes.Count &&
                                        System.Text.Encoding.UTF8.GetString(binaryDataBytes.ToArray(), i, contentKeyword.Length) == contentKeyword)
                                    {
                                        contentStartIndex = i + contentKeyword.Length;
                                        headerLength = contentStartIndex;
                                        headers = System.Text.Encoding.UTF8.GetString(binaryDataBytes.ToArray(), 0, headerLength - contentKeyword.Length);
                                        break;
                                    }
                                }

                                // Extract the binary data after the headers
                                if (contentStartIndex != -1)
                                {
                                    int binaryDataLength = binaryDataBytes.Count - contentStartIndex;
                                    newImgData = new byte[binaryDataLength];
                                    Array.Copy(binaryDataBytes.ToArray(), contentStartIndex, newImgData, 0, binaryDataLength);

                                    // Save the binary data to a .tgz file
                                    string filePath = "D:\\CSharpApplications\\repos\\AttendancePUSHSDK\\Camera\\getData\\"+fName.Text;

                                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                                    {
                                        fileStream.Write(newImgData, 0, newImgData.Length);
                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader reader = new StreamReader(body, context.Request.ContentEncoding))
                                {
                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        okcnt++;
                                        if (line.Length == 0)
                                        {
                                            continue;
                                        }
                                        if (line.StartsWith("~DeviceName") && line.Contains("PushVersion"))
                                        {
                                            SetConfParameter(line);
                                            break;
                                        }
                                        else if (line.StartsWith("PushVersion") || line.StartsWith("~DeviceName") || line.Contains("MAC") || line.StartsWith("UserCount") || line.StartsWith("FaceCount") || line.Contains("FPCount") || line.Contains("FvCount") || line.StartsWith("PvCount") || line.StartsWith("IPAddress") || line.StartsWith("~Platform"))
                                        {
                                            SetConfParameter(line);
                                            snflag = 0;
                                        }
                                        else if (line.StartsWith("PIN=") || line.StartsWith("BIOPHOTO PIN="))
                                        {
                                            if (line.StartsWith("BIOPHOTO PIN="))
                                            {
                                                string pinPattern = "PIN=";

                                                int pinIndex = line.IndexOf(pinPattern);
                                                if (pinIndex != -1)
                                                {
                                                    int startIndex = pinIndex + pinPattern.Length;
                                                    int endIndex = line.IndexOfAny(new char[] { '\t', ' ', '\n', '\r' }, startIndex);
                                                    if (endIndex == -1)
                                                    {
                                                        endIndex = line.Length;
                                                    }

                                                    pin = line.Substring(startIndex, endIndex - startIndex).Trim();
                                                }
                                            }
                                            else
                                            {
                                                pin = line.Substring("PIN=".Length).Trim();
                                            }
                                        }
                                        else if (line.StartsWith("SN="))
                                        {
                                            serialNumber = line.Substring("SN=".Length).Trim();
                                        }
                                        else if (line.StartsWith("size="))
                                        {
                                            size = line.Substring("size=".Length).Trim();
                                        }
                                        else if (line.Contains("Return") && okcnt == 1)
                                        {
                                            devData = 0;
                                            if (usrInfoFlag == 1 && LogText.Text.Length > 0)
                                            {
                                                populateUserData();
                                            }
                                            int startIndex = line.IndexOf("Return=");

                                            if (startIndex != -1)
                                            {
                                                startIndex += "Return=".Length;

                                                int endIndex = line.IndexOf('&', startIndex);
                                                if (endIndex == -1)
                                                {
                                                    endIndex = line.Length;
                                                }

                                                string returnValue = line.Substring(startIndex, endIndex - startIndex);
                                                returnInt = int.Parse(returnValue);
                                            }
                                            SetResponseBox(line);
                                        }

                                        if (line.StartsWith("BIOPHOTO"))
                                        {
                                            string pinPattern = "Content=";
                                            int lastIndex = line.LastIndexOf(pinPattern) + pinPattern.Length;
                                            if (lastIndex != -1 && lastIndex < line.Length - 1)
                                            {
                                                try
                                                {
                                                    string base64str = line.Substring(lastIndex);
                                                    Size boxSize = picBox.Size;
                                                    Image image = viewImage(base64str);
                                                    Bitmap resizedImage = ResizeImage(image, boxSize);
                                                    picBox.Image = image;
                                                    picBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                                }
                                                catch (ArgumentException ex)
                                                {
                                                    SetMsgBox("Error loading image: " + ex.Message);
                                                }
                                                //done = 0;
                                            }
                                        }
                                        SetLogTextBox(line);
                                        /*if (done == 1){ break; }*/
                                    }
                                }

                            }
                        }
                    }
                }
                SetLogTextBox("\n\r");
                SetMsgBox("\n\r");
                StringBuilder responseBuilder = new StringBuilder();

                if (request.HttpMethod == "GET")
                {
                    string responseBody = "";
                    if (request.Url.AbsolutePath == "/iclock/cdata")
                    {
                        msgBody = 1;
                    }
                    else if (request.Url.AbsolutePath == "/iclock/getrequest" || updFacFlg > 0/*|| request.RawUrl == "//mnt/mtdblock/data/biophoto/face"*/)
                    {
                        msgBody = 2;
                        if (faceFlag == 1)
                        {
                            faceFlag = 0;
                        }
                        if (btnClientInfo.Enabled == true)
                        {
                            btnClInf = 1;
                        }
                    }
                    else if (request.Url.AbsolutePath.Contains("/Camera/sendFile"))
                    {
                        if(request.Url.AbsolutePath.EndsWith(".tgz") || request.Url.AbsolutePath.EndsWith(".jpeg"))
                        {
                            responseBody = "-404";
                        }
                        else
                        {
                            responseBody = fileContentString;
                        }
                        
                    }
                    else
                    {
                        msgBody = 0;
                    }                    
                    if(updFacFlg == 0 && request.Url.AbsolutePath.Contains("/Camera/") == false)
                    {
                        if ((index != -1 && serialNumberSelected != null) && (serialNumberSelected == SerialNumber))
                        {
                            // Generate configuration information based on the request parameters
                            responseBody = GenerateConfigurationInfo(context, serialNumberSelected, msgBody);
                        }
                        else if(serialNumberSelected == "")
                        {
                            responseBody = "";
                        }
                        else
                        {
                            responseBody = "OK";
                        }
                    }
                    if(responseBody != "")
                    {
                        // Set response headers`
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.AddHeader("Server", "nginx/1.6.0");
                        DateTime date = DateTime.Now;
                        string outputDateTimeString = date.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture);
                        context.Response.AddHeader("Date", outputDateTimeString);//DateTime.UtcNow.ToString("R"));
                        if ((updFacFlg == 1 && request.RawUrl.StartsWith("/iclock/getrequest") == false) || request.Url.AbsolutePath.EndsWith(".jpeg"))
                        {
                            context.Response.AddHeader("Content-Type", "image/jpeg");
                        }
                        else if(request.Url.AbsolutePath.EndsWith(".tgz"))
                        {
                            context.Response.AddHeader("Content-Type", "application/octet-stream");
                        }
                        else
                        {
                            context.Response.AddHeader("Content-Type", "text/plain");
                        }
                        context.Response.AddHeader("Connection", "close");
                        context.Response.AddHeader("Pragma", "no-cache");
                        context.Response.AddHeader("Cache-Control", "no-store");

                        // Calculate content length based on the length of the configuration information
                        int contentLength;
                        byte[] resultBytes = null;
                        if (updFacFlg > 0 && request.RawUrl.StartsWith("/iclock/getrequest") == false)
                        {
                            System.Drawing.Image img = System.Drawing.Image.FromFile("C:\\Users\\himanshu\\Pictures\\Camera\\sam.jpg");
                            MemoryStream ms = new MemoryStream();
                            img.Save(ms, img.RawFormat);
                            byte[] msg = ms.ToArray();
                            context.Response.ContentLength64 = msg.Length;
                            context.Response.OutputStream.Write(msg, 0, msg.Length);
                        }
                        else
                        {
                            if(responseBody == "-404")
                            {
                                context.Response.OutputStream.Write(fileContentBytes, 0, fileContentBytes.Length);
                            }
                            else
                            {
                                // Calculate content length based on the length of the configuration information
                                contentLength = Encoding.UTF8.GetByteCount(responseBody);
                                context.Response.ContentLength64 = contentLength;
                                byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                            }
                        }

                        // Get the response headers collection
                        var headers = context.Response.Headers;

                        // Initialize a StringBuilder to build the message string
                        StringBuilder headerStringBuilder = new StringBuilder();

                        // Iterate over all headers and append them to the StringBuilder
                        foreach (string key in headers.AllKeys)
                        {
                            string val = headers[key];
                            headerStringBuilder.AppendLine($"{key}: {val}");
                        }

                        SetMsgBox(headerStringBuilder.ToString());  
                        if(responseBody != "-404")
                        {
                            if (request.Url.AbsolutePath.EndsWith("C:/Users/himanshu/Pictures/Camera") == false)
                            {
                                SetMsgBox("\n" + responseBody + "\n");
                            }
                            else
                            {
                                // Convert byte array to a hexadecimal string
                                string byteString = BitConverter.ToString(resultBytes);
                                SetMsgBox(byteString);
                            }
                        }

                        btnClInf = 0;
                        if (btnClientInfo.Enabled == true)
                        {
                            btnClientCounter++;
                        }
                    }

                }
                else if (request.HttpMethod == "POST")
                {
                    string responseBody;
                    if ((index != -1 && serialNumberSelected != null) && (serialNumberSelected == SerialNumber))
                    {
                        if (request.Url.AbsolutePath == "/iclock/cdata" || request.Url.AbsolutePath == "/iclock/fdata" || request.Url.AbsolutePath == "/iclock/devicecmd" || request.Url.AbsolutePath == "/iclock/registry" || request.Url.AbsolutePath == "/iclock/ping")
                        {
                            msgBody = 0;
                            if (request.Url.AbsolutePath == "/iclock/registry" || request.Url.AbsolutePath == "/iclock/ping" || request.Url.AbsolutePath == "/iclock/fdata")
                            {
                                devData = 0;
                            }
                            if ((faceFlag == 1 || request.Url.AbsolutePath == "/iclock/devicecmd"))
                            {
                                devData = 0;
                                faceFlag = 0;
                            }
                            if (!btnClientInfo.Enabled && request.Url.AbsolutePath == "/iclock/devicecmd")
                            {
                                btnClientInfo.Invoke((MethodInvoker)(() => btnClientInfo.Enabled = true));
                                btnClInf = 0;
                            }
                            if (updFacFlg > 0)
                            {
                                msgBody = 2;
                                SetRadioValue(updFace, false);
                                updFacFlg = 0;
                                updUserInfo.Enabled = true;
                            }
                            if(gfile == 1 && request.Url.AbsolutePath == "/iclock/devicecmd")
                            {
                                SetButtonEnabled(getFile, true);gfile = 0;
                            }
                        }
                        else
                        {
                            okcnt = 1;
                        }
                        // Generate configuration information based on the request parameters
                        responseBody = GenerateConfigurationInfo(context, serialNumberSelected, msgBody);
                    }
                    else if (serialNumberSelected == "")
                    {
                        responseBody = "";
                    }
                    else
                    {
                        responseBody = "OK";                        
                    }
                    if(responseBody != "")
                    {
                        // Set response headers
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.AddHeader("Server", "nginx/1.6.0");
                        DateTime date = DateTime.Now;
                        string formattedDate = date.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture);
                        //string formattedDate = now.ToString("ddd, dd MMM yyyy hh:mm:ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture);
                        context.Response.AddHeader("Date", formattedDate);// DateTime.UtcNow.ToString("R"));
                        context.Response.AddHeader("Content-Type", "text/plain");
                        context.Response.AddHeader("Connection", "close");
                        context.Response.AddHeader("Pragma", "no-cache");
                        context.Response.AddHeader("Cache-Control", "no-store");

                        // Calculate content length based on the length of the configuration information
                        int contentLength = Encoding.UTF8.GetByteCount(responseBody);
                        context.Response.ContentLength64 = contentLength;

                        // Write the configuration information to the response stream
                        byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                        SetMsgBox(responseBody);
                    }
                    setResultBoxValue(succkey);
                }
                else
                {
                    // Invalid request, send a 404 response
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                // Close the response after handling the request
                context.Response.Close();
                if (reboot.Checked == true)
                {
                    reboot.Checked = false;
                    rboot = 0;
                    SetControlsReadOnly(setConfig, false);
                }
                if (reload.Checked == true)
                {
                    reload.Checked = false;
                    rload = 0;
                    SetControlsReadOnly(setConfig, false);
                }
                if (!btnClientInfo.Enabled)
                {
                    btnClInf = 0;
                    if (btnClientCounter == 3)
                    {
                        btnClientInfo.Enabled = true;
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        public string GetCheckedCheckBoxNames(System.Windows.Forms.GroupBox groupBox)
        {
            List<string> checkedNames = new List<string>();

            foreach (Control control in groupBox.Controls)
            {
                if (control is System.Windows.Forms.CheckBox checkBox && checkBox.Checked)
                {
                    checkedNames.Add(checkBox.Text);
                }
            }

            checkedNames.Reverse(); // Reverse the list in-place
            return string.Join(" ", checkedNames);
        }

        public string getTransTimes()
        {
            string s = "";
            int cnt = transTimes.Items.Count;
            for(int i= 0; i<cnt;i++)
            {
                if(i!=cnt-1)
                {
                    s += transTimes.Items[i].ToString() + ";";continue;
                }
                s += transTimes.Items[i].ToString();
            }
            return s;
        }

        public string GenerateServerResponse(string SN)
        {
            StringBuilder responseBuilder = new StringBuilder();
            // Response body
            responseBuilder.AppendLine("GET OPTION FROM:"+ SN);
            responseBuilder.AppendLine("ATTLOGStamp=None");
            responseBuilder.AppendLine("OPERLOGStamp=9999");
            responseBuilder.AppendLine("ATTPHOTOStamp=None");
            responseBuilder.AppendLine("ErrorDelay="+errDelay.Text.ToString());
            responseBuilder.AppendLine("Delay="+delay.Text.ToString());
            string transTimes = getTransTimes();
            DateTime dateTime = DateTime.Now; 
            responseBuilder.AppendLine("TransTimes="+ /*dateTime.Hour+":"+dateTime.Minute);//+*/ transTimes);
            responseBuilder.AppendLine("TransInterval="+ transInt.Text.ToString());
            responseBuilder.AppendLine("TransFlag="+ GetCheckedCheckBoxNames(transFlag));
            responseBuilder.AppendLine("TimeZone=330");
            responseBuilder.AppendLine("Realtime="+realTime.Checked);
            responseBuilder.AppendLine("Encrypt=None");

            return responseBuilder.ToString();
        }
        private string GenerateConfigurationInfo(HttpListenerContext context, string serialNumber, int res)
        {
            StringBuilder responseBuilder = new StringBuilder();
            string configurationInfo = "";
            if (res == 0)
            {
                configurationInfo = "OK";
                if (devData == 1)
                {
                    configurationInfo += ":" + okcnt;
                }
            }
            else if (res == 2)
            {
                configurationInfo = getCommand();
                if (configurationInfo == "")
                {
                    configurationInfo = "OK";
                }
                if(configurationInfo != "OK" && configurationInfo != "")
                {
                    SetCmdHstBox(cmdStr);
                }
                cmdStr = "";
            }
            else
            {
                // Calculate the content length based on the length of the configuration information
                configurationInfo = GenerateServerResponse(serialNumber);
            }

            // Append the configuration information
            responseBuilder.Append(configurationInfo);

            return responseBuilder.ToString();
        }

        private void serverIP_TextChanged(object sender, EventArgs e)
        {

        }

        private void portNo_TextChanged(object sender, EventArgs e)
        {
            if (portNo.Text.Length == 0)
                return;
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            try
            {
                listener?.Stop();
                listener.Close();
                //msgBox.Clear();
                msgBox.Text += "\r\nServer stopped.";
                stopBtn.Enabled = false;
                startBtn.Enabled = true;
                SetControlsReadOnly(setConfig, false);
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                }
                StrtStpVideoBtn.Text = "Start Video";
            }
            catch (Exception ex)
            {
                msgBox.Text += "\r\nException:" + ex.Message;
                stopBtn.Enabled = false;
                startBtn.Enabled = true;
            }
        }

        private void msgBox_TextChanged(object sender, EventArgs e)
        {
            // Record the current selection start position
            int selectionStart = msgBox.SelectionStart;

            // Set the selection start position back to the end of the TextBox
            msgBox.SelectionStart = msgBox.Text.Length;

            // Scroll to the end of the TextBox
            msgBox.ScrollToCaret();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                count = 1;
            }
            else
            {
                count = 0;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                count = 1;
            }
            else
            {
                count = 0;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                count = 1;
            }
            else
            {
                count = 0;
            }
        }

        private void attPhoto_CheckedChanged(object sender, EventArgs e)
        {
            if (attPhoto.Checked)
            {
                count = 1;
            }
            else
            {
                count = 0;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                count = 1;
            }
            else
            {
                count = 0;
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                count = 1;
            }
            else
            {
                count = 0;
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                count = 1;
            }
            else
            {
                count = 0;
            }
        }
        private static string CleanBase64String(string base64Data)
        {
            // Remove any non-Base64 characters
            base64Data = Regex.Replace(base64Data, @"[^a-zA-Z0-9+/=]", string.Empty);

            // Add padding if necessary
            int padding = 4 - (base64Data.Length % 4);
            if (padding < 4)
            {
                base64Data += new string('=', padding);
            }

            return base64Data;
        }

        private static Image viewImage(string base64Data)
        {
            // Ensure the Base64 string is valid and properly formatted
            base64Data = CleanBase64String(base64Data);
            byte[] imageBytess = Convert.FromBase64String(base64Data);

            // Convert byte array to Image
            using (var ms = new MemoryStream(imageBytess))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        // Function to resize image according to the picture box dimensions
        public Bitmap ResizeImage(Image imgToResize, Size boxSize)
        {
            // Get the original dimensions
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            // Calculate the aspect ratio
            float nPercentW = (float)boxSize.Width / (float)sourceWidth;
            float nPercentH = (float)boxSize.Height / (float)sourceHeight;

            // Use the smaller scaling factor to ensure the image fits in the box
            float nPercent = (nPercentH < nPercentW) ? nPercentH : nPercentW;

            // Calculate the new dimensions
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            // Create a new bitmap with the new dimensions
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(b);

            // Set high-quality resizing options
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return b;
        }

        // NewFrame event handler
/*      void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Get the new frame
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

            // Display the frame in the PictureBox
            picBox.Image = bitmap;
            picBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }*/
        /*private void button2_Click(object sender, EventArgs e)
        {
            if (!startBtn.Enabled)
            { 
                if(StrtStpVideoBtn.Text == "Start Video")
                {
                    // Get the available video devices
                    videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                    if (videoDevices.Count > 0)
                    {
                        // Select the first available device
                        videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                        // Set the NewFrame event handler
                        videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);

                        // Start the video source
                        videoSource.Start();
                    }
                    else
                    {
                        MessageBox.Show("No video devices found.");
                    }
                    StrtStpVideoBtn.Text = "Stop Video";
                }
                else
                {
                    if (videoSource != null && videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();
                        videoSource.WaitForStop();
                    }
                    StrtStpVideoBtn.Text = "Start Video";
                }
            }
        }*/

        private void button2_Click(object sender, EventArgs e)
        {
            if (!startBtn.Enabled)
            {
                if (StrtStpVideoBtn.Text == "Start Video")
                {
                    // Get the available video devices
                    videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                    if (videoDevices.Count > 0)
                    {
                        // Select the first available device
                        videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                        // Set the NewFrame event handler
                        videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);

                        // Start the video source
                        videoSource.Start();
                    }
                    else
                    {
                        MessageBox.Show("No video devices found.");
                    }
                    StrtStpVideoBtn.Text = "Stop Video";
                }
                else
                {
                    if (videoSource != null && videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();
                        videoSource.WaitForStop();
                    }
                    StrtStpVideoBtn.Text = "Start Video";
                }
            }
        }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Get the current frame
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();

            // Flip the frame horizontally to create a mirror image
            frame.RotateFlip(RotateFlipType.RotateNoneFlipX);

            // Display the frame in a PictureBox or handle it as needed
            picBox.Image = frame;
            picBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        private void usrPin_TextChanged(object sender, EventArgs e)
        {
            if (usrPin.Text.StartsWith("Enter"))
            {
                usrPin.Clear();
            }
        }
        private void allUsr_CheckedChanged(object sender, EventArgs e)
        {
            if(allUser.Checked)
            {
                usrPin.Text = "Enter UserPIN";
                usrPin.Enabled = false;
                btnClInf = 1; 
            }
            else
            {
                usrPin.Enabled = true; 
            }
            selectUserPIN.Enabled = true;
            selectUserPIN.Items.Clear();
            selectUserPIN.Enabled = false;
        }
        private void selectUsrPin_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        private void label19_Click(object sender, EventArgs e)
        {

        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void getUsrData_Click(object sender, EventArgs e)
        {
            if(selectUserPIN.SelectedIndex == -1)
            {
                return;
            }
            string selectedValue = selectUserPIN.SelectedItem.ToString();
            if(int.TryParse(selectedValue, out _))
            {
                fillUserSpecificDetails(selectUserPIN.SelectedIndex+1);
            }
        }
        private void rmtPinBox_TextChanged(object sender, EventArgs e)
        {
            if (isHandlingTextChanged)
                return;

            isHandlingTextChanged = true;

            if (string.IsNullOrWhiteSpace(rmtPinBox.Text))
            {
                rmtPinBox.Text = "Enter UserPIN";
            }
            else if (rmtPinBox.Text.StartsWith("Enter") && rmtPinBox.Text.Length != 0)
            {
                rmtPinBox.Clear();
            }

            isHandlingTextChanged = false;
        }

        private void SNComboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            selectUserPIN.Items.Clear();
            if ((SNComboBox.Items.Count > 0) && (SNComboBox.SelectedIndex != -1))
            {
                //LabelSNSelect.Visible = false;
                LabelSNSelect.ForeColor = Color.Gray;
                if (dftsPinVal != "")
                {
                    snflag = 1;
                }
            }
            dftsPinVal = SNComboBox.Text;
        }

        private void fidBox_TextChanged(object sender, EventArgs e)
        {
            if (fidBox.Text.StartsWith("Enter"))
            {
                fidBox.Clear();
            }
        }

        private void retryCnt_TextChanged(object sender, EventArgs e)
        {
            if (retryCnt.Text.StartsWith("Retr"))
            {
                retryCnt.Clear();
            }
        }

        private void overWrite_TextChanged_1(object sender, EventArgs e)
        {
            if (overWrite.Text.StartsWith("Over"))
            {
                overWrite.Clear();
            }
        }

        private void rmtEnrollFP_CheckedChanged(object sender, EventArgs e)
        {
            if(rmtEnrollFP.Checked)
            {
                bioType.Enabled = false;
            }
        }

        private void rmtEnrollFP_CheckedChanged_1(object sender, EventArgs e)
        {
            if(rmtEnrollFP.Checked)
            {
                fidBox.ReadOnly = false;
                panelRmtPara.Enabled = true;
                bioType.Enabled = false;
                enterCard.Enabled = false;
                fidBox.Enabled = true;
                retryCnt.Enabled = true;    
                overWrite.Enabled = true;                
            }
        }

        private void enterCard_TextChanged(object sender, EventArgs e)
        {

        }

        private void enrollCard_CheckedChanged(object sender, EventArgs e)
        {
            if(enrollCard.Checked)
            {
                panelRmtPara.Enabled = true;
                fidBox.Enabled = false;
                bioType.Enabled = false;
                retryCnt.Enabled = true;
                overWrite.Enabled = false;
                enterCard.Enabled = false;
            }
        }

        private void enrollFace_CheckedChanged(object sender, EventArgs e)
        {
            if(enrollFace.Checked)
            {
                panelRmtPara.Enabled = true;
                fidBox.Enabled = true;
                bioType.Enabled = false;
                retryCnt.Enabled = true;
                overWrite.Enabled = true;
                enterCard.Enabled = false;
                SetTextBoxValue(fidBox, "111");
                fidBox.ReadOnly = true;
            }
        }

        private void enrollPalm_CheckedChanged(object sender, EventArgs e)
        {
            if (enrollPalm.Checked)
            {
                panelRmtPara.Enabled = true;
                fidBox.Enabled = false;
                bioType.Enabled = true;
                retryCnt.Enabled = true;
                overWrite.Enabled = true;
                enterCard.Enabled = true;
            }
        }
        private void DisablePanelsInGroupBox(GroupBox groupBox)
        {
            foreach (Control control in groupBox.Controls)
            {
                if (control is Panel)
                {
                    control.Enabled = false;
                }
            }
        }

        private void loadInfo_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void verify1to1_CheckedChanged(object sender, EventArgs e)
        {
            if (verify1to1.Checked)
            {
                fidBox.Text = "20";
                retryCnt.Text = "1";
                overWrite.Text = "0";
                retryCnt.Enabled = false;
                overWrite.Enabled = false;
                fidBox.Enabled = false;
                fidBox.ReadOnly = true;
            }
            else
            {
                overWrite.Enabled = true;
                retryCnt.Enabled = true;
                fidBox.Enabled = true;
                fidBox.ReadOnly = false;
            }
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void cmdID_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void setOpt_Click(object sender, EventArgs e)
        {
            if (optionSetBox.Text != "")
            {
                cmdStr = ":SET OPTION " + optionSetBox.Text;
            }
        }

        private void cmdhstBox_TextChanged(object sender, EventArgs e)
        {
            // Record the current selection start position
            int selectionStart = cmdhstBox.SelectionStart;

            // Set the selection start position back to the end of the TextBox
            cmdhstBox.SelectionStart = cmdhstBox.Text.Length;
            // Scroll to the end of the TextBox
            cmdhstBox.ScrollToCaret();
        }

        private void EnableVerify_Click(object sender, EventArgs e)
        {
            if (EnableVerify.Text == "Enable")
            {
                EnableVerify.Text = "Disable";
                EnableVerify.ForeColor = Color.White;
                EnableVerify.BackColor = Color.Red;
                cmdStr = ":SET OPTION VerifyStatus=1";
            }
            else 
            {
                EnableVerify.Text = "Enable";
                EnableVerify.ForeColor = Color.White;
                EnableVerify.BackColor = Color.Green;
                cmdStr = ":SET OPTION VerifyStatus=0";
            }
        }

        private void rmtEnrollBox_Enter(object sender, EventArgs e)
        {
            foreach (Control control in rmtEnrollBox.Controls)
            {
                if (control is Panel)
                {
                    control.Enabled = false;
                }
            }
        }

        private void sndRmtEnrollmentCmd_Click(object sender, EventArgs e)
        {
            if (sndRmtEnrollmentCmd.Enabled)
            {
                sndRmtEnrollmentCmd.Enabled = false;
            }
        }

        private void updFace_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void bioType_TextChanged(object sender, EventArgs e)
        {

        }

        private void getFile_Click(object sender, EventArgs e)
        {
            gfile = 1;
            if (getFile.Enabled)
            {
                getFile.Enabled = false;
            }
        }
        private void sendFile_Click(object sender, EventArgs e)
        {
            sfile = 1;
            if (sendFile.Enabled)
            {
                sendFile.Enabled = false;
            }

            // Create an instance of OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog.Filter = "Text Files (.txt)|*.txt|TGZ Files (.tgz)|*.tgz|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            // Set OpenFileDialog title
            openFileDialog.Title = "Select a File";

            // Show the dialog and get result.
            DialogResult result = openFileDialog.ShowDialog();

            // Process open file dialog box results 
            if (result == DialogResult.OK)
            {
                // Get the file path as returned by the dialog
                filePath = openFileDialog.FileName;

                // Extract the file name from the path if needed
                int lastSlashIndex = filePath.LastIndexOf('\\');
                fileName = filePath.Substring(lastSlashIndex + 1);

                // Read file content in binary format
                fileContentBytes = File.ReadAllBytes(filePath);

                // Convert byte array to string (assuming the file content is text)
                fileContentString = System.Text.Encoding.Default.GetString(fileContentBytes);
            }
            else
            {
                sendFile.Enabled = true;
            }
        }

        private void updUserInfo_Click(object sender, EventArgs e)
        {
            if(updUserInfo.Enabled)
            {
                updUserInfo.Enabled = false;
            }
        }

        private void responseBox_TextChanged(object sender, EventArgs e)
        {
            // Record the current selection start position
            int selectionStart = responseBox.SelectionStart;

            // Set the selection start position back to the end of the TextBox
            responseBox.SelectionStart = responseBox.Text.Length;

            // Scroll to the end of the TextBox
            responseBox.ScrollToCaret();
        }

        void disableAllParameter(bool value)
        {
            cmdID.Enabled = value;
            usrNamBox.Enabled = value;
            privilegeBox.Enabled = value;
            passwd.Enabled = value;
            crdBox.Enabled = value;
            grpBox.Enabled = value;
            tzBox.Enabled = value;
            verifyBox.Enabled = value;
            vcBox.Enabled = value;
            StrtDatTimBox.Enabled = value;
            EndDatTimeBox.Enabled = value;
            noBox.Enabled = value;
            indexBox.Enabled = value;
            validBox.Enabled = value;
            duressBox.Enabled = value;
            typBox.Enabled = value;
            MajVerBox.Enabled = value;
            MinVerBox.Enabled = value;
            frmtBox.Enabled = value;
            fnBox.Enabled = value;
            szBox.Enabled = value;
            fnBox1.Enabled = value;
            type1.Enabled = value;
            sz1.Enabled = value;
        }


        private void updUsrPic_CheckedChanged(object sender, EventArgs e)
        {
            disableAllParameter(false);
        }

        private void updUsrInf_CheckedChanged(object sender, EventArgs e)
        {
            disableAllParameter(false);
            EnterUpdateValue(1);
        }
        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }
        private string ImageToBase64String(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[] in specified format
                image.Save(ms, format);
                imageBytes = ms.ToArray();
                File.WriteAllBytes("C:\\Users\\himanshu\\Pictures\\Camera\\sam.jpeg", imageBytes);
                // Convert byte[] to Base64 string
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        private void clckImg_Click(object sender, EventArgs e)
        {
            // Check if there is an image in the PictureBox
            if (picBox.Image != null)
            {
                // Convert the image to a Base64 string
                ImageString = ImageToBase64String(picBox.Image, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                MessageBox.Show("No image found in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
            StrtStpVideoBtn.Text = "Start Video";
        }

        private void LogText_TextChanged(object sender, EventArgs e)
        {
            // Record the current selection start position
            int selectionStart = LogText.SelectionStart;

            // Set the selection start position back to the end of the TextBox
            LogText.SelectionStart = LogText.Text.Length;

            // Scroll to the end of the TextBox
            LogText.ScrollToCaret();
        }

        private void btnClientInfo_Click(object sender, EventArgs e)
        {
            if (!startBtn.Enabled)
            {
                if (!allUser.Checked && usrPin.Enabled && usrPin.Text == "Enter UserPIN")
                {
                    return;
                }
                if (btnClientInfo.Enabled)
                {
                    LogText.Clear();
                    btnClientInfo.Enabled = false;
                    getUsrData.Enabled = true;
                }
                else
                {
                    getUsrData.Enabled = true;
                }
                if (allUser.Checked)
                {
                    btnClInf = 0;
                    btnClientCounter = 1;
                    return;
                }
                bool isValidNumber = int.TryParse(usrPin.Text.ToString(), out _);
                if (isValidNumber && usrPin.Enabled)
                {
                    btnClInf = 1;
                }
            }
        }
        private void reload_CheckedChanged(object sender, EventArgs e)
        {
            if (reload.Checked)
                rload = 1;
            else rload = 0;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void reboot_CheckedChanged(object sender, EventArgs e)
        {
            if (reboot.Checked)
                rboot = 1;
            else rboot = 0;
        }
        private void addTransTimes_Click(object sender, EventArgs e)
        {
            // Ensure both ComboBoxes have selected values
            if (string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrEmpty(comboBox2.Text))
            {
                SetMsgBox("\r\nPlease select both hour and minute.");
                return;
            }

            // Concatenate hour and minute with a colon
            string time = $"{comboBox1.Text.Trim()}:{comboBox2.Text.Trim()}";

            // Check if the time already exists in the transTimes ComboBox
            if (!transTimes.Items.Contains(time))
            {
                // Add the time to the transTimes ComboBox
                transTimes.Items.Add(time);

                // Select the newly added item
                transTimes.SelectedItem = time;
            }
            else
            {
                SetMsgBox("\r\nThis time already exists in the list.");
            }
        }
    }
}
