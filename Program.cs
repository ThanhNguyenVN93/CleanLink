using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CleanLink
{
    public class MainForm : Form
    {
        private TextBox txtUrl;
        private Button btnOpen;
        private Button btnPasteOpen;
        private Button btnClear;
        private Label lblStatus;

        public MainForm()
        {
            Text = "CleanLink (.NET 3.5)";
            ClientSize = new Size(520, 150);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            try
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch
            {
            }

            txtUrl = new TextBox();
            txtUrl.Location = new Point(12, 12);
            txtUrl.Size = new Size(496, 22);

            btnOpen = new Button();
            btnOpen.Text = "Xóa khoảng trắng & Mở";
            btnOpen.Location = new Point(12, 46);
            btnOpen.Size = new Size(170, 30);
            btnOpen.Click += new EventHandler(btnOpen_Click);

            btnPasteOpen = new Button();
            btnPasteOpen.Text = "Dán từ Clipboard & Mở";
            btnPasteOpen.Location = new Point(190, 46);
            btnPasteOpen.Size = new Size(170, 30);
            btnPasteOpen.Click += new EventHandler(btnPasteOpen_Click);

            btnClear = new Button();
            btnClear.Text = "Xóa";
            btnClear.Location = new Point(368, 46);
            btnClear.Size = new Size(80, 30);
            btnClear.Click += new EventHandler(btnClear_Click);

            lblStatus = new Label();
            lblStatus.Location = new Point(12, 92);
            lblStatus.Size = new Size(496, 44);
            lblStatus.Text = "";

            Controls.Add(txtUrl);
            Controls.Add(btnOpen);
            Controls.Add(btnPasteOpen);
            Controls.Add(btnClear);
            Controls.Add(lblStatus);

            AcceptButton = btnOpen;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            CleanAndOpen(txtUrl.Text);
        }

        private void btnPasteOpen_Click(object sender, EventArgs e)
        {
            string text = Clipboard.GetText();
            txtUrl.Text = text;
            CleanAndOpen(text);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUrl.Clear();
            lblStatus.Text = "";
            txtUrl.Focus();
        }

        private void CleanAndOpen(string raw)
        {
            string cleanUrl;
            string error;
            if (!Program.CleanAndValidateUrl(raw, out cleanUrl, out error))
            {
                lblStatus.ForeColor = Color.Firebrick;
                lblStatus.Text = error;
                return;
            }

            txtUrl.Text = cleanUrl;
            lblStatus.ForeColor = Color.DimGray;
            lblStatus.Text = "Đang kiểm tra link...";
            Application.DoEvents();

            if (!Program.CheckUrlAlive(cleanUrl, out error))
            {
                lblStatus.ForeColor = Color.Firebrick;
                lblStatus.Text = error;
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(cleanUrl) { UseShellExecute = true });
                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = "Đã mở: " + cleanUrl;
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Firebrick;
                lblStatus.Text = "Lỗi khi mở: " + ex.Message;
            }
        }
    }

    public static class Program
    {
        public static bool CleanAndValidateUrl(string raw, out string cleanUrl, out string error)
        {
            cleanUrl = Regex.Replace(raw ?? "", @"\s+", "");
            error = null;

            if (cleanUrl.Length == 0)
            {
                error = "URL trống.";
                return false;
            }

            Uri uri;
            if (!Uri.TryCreate(cleanUrl, UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                error = "URL không hợp lệ (cần http:// hoặc https://).";
                return false;
            }

            string host = uri.Host ?? "";
            bool isLocal = host.Equals("localhost", StringComparison.OrdinalIgnoreCase);
            if (!isLocal && (!host.Contains(".") || host.StartsWith(".") || host.EndsWith(".") || host.Contains("..")))
            {
                error = "URL không hợp lệ (tên miền thiếu phần mở rộng, ví dụ .com).";
                return false;
            }

            return true;
        }

        public static bool CheckUrlAlive(string url, out string error)
        {
            error = null;
            Uri uri = new Uri(url);

            try
            {
                IPAddress[] addrs = Dns.GetHostAddresses(uri.Host);
                if (addrs == null || addrs.Length == 0)
                {
                    error = "Tên miền không tồn tại (DNS lỗi).";
                    return false;
                }
            }
            catch
            {
                error = "Tên miền không tồn tại hoặc không phân giải được DNS.";
                return false;
            }

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.Method = "HEAD";
                req.Timeout = 4000;
                req.ReadWriteTimeout = 4000;
                req.AllowAutoRedirect = true;
                req.UserAgent = "Mozilla/5.0";
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    int code = (int)resp.StatusCode;
                    if (code == 404 || code == 410)
                    {
                        error = "Link không tồn tại (HTTP " + code + ").";
                        return false;
                    }
                    return true;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse resp = ex.Response as HttpWebResponse;
                if (resp != null)
                {
                    int code = (int)resp.StatusCode;
                    if (code == 404 || code == 410)
                    {
                        error = "Link không tồn tại (HTTP " + code + ").";
                        return false;
                    }
                    // 401, 403, 405, etc. mean the server is alive
                    return true;
                }

                error = "Trang web không phản hồi (die/timeout).";
                return false;
            }
            catch
            {
                error = "Không thể kết nối tới trang web.";
                return false;
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                string raw = string.Join(" ", args);
                string cleanUrl;
                string error;
                if (CleanAndValidateUrl(raw, out cleanUrl, out error) && CheckUrlAlive(cleanUrl, out error))
                {
                    Process.Start(new ProcessStartInfo(cleanUrl) { UseShellExecute = true });
                    return;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
