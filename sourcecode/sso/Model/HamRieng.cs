using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace SSOModels
{
    public class HamRieng
    {
        public static void SendMail(String MailFrom, String PassMailFrom, String MailServer, String MailTo, String CC, String BCC, String Subject, String Body, String[] attach)
        {
            //Lay dia chi basicCredential
            Char[] ch = { '@' };
            String[] tmp;
            String mailfrom;
            tmp = MailFrom.Split(ch);
            mailfrom = tmp[0];
            //Co che chung thuc cua Mang
            NetworkCredential basicCredential = new NetworkCredential(mailfrom, PassMailFrom);
            SmtpClient client = new SmtpClient(MailServer, 25);
            MailMessage mail = new MailMessage();
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential;
            //Mail Gửi
            mail.From = new MailAddress(MailFrom);
            //Mail nhận
            String[] a;
            a = MailTo.Split(',');
            if (a.Length > 1)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] != "")
                    {
                        mail.To.Add(new MailAddress(a[i]));
                    }
                }

            }
            else
            {
                mail.To.Add(new MailAddress(MailTo));
            }
            //Gửi CC
            if (string.IsNullOrEmpty(CC) == false)
            {
                String[] c;
                c = CC.Split(',');
                if (c.Length > 1)
                {
                    for (int i = 0; i < c.Length; i++)
                    {
                        if (c[i] != "")
                        {
                            mail.CC.Add(new MailAddress(c[i]));
                        }
                    }
                }
                else
                {
                    mail.CC.Add(new MailAddress(CC));
                }
            }
            //Gửi BCC
            if (string.IsNullOrEmpty(BCC) == false)
            {
                String[] b;
                b = BCC.Split(',');
                if (b.Length > 1)
                {
                    for (int i = 0; i < b.Length; i++)
                    {
                        if (b[i] != "")
                        {
                            mail.Bcc.Add(new MailAddress(b[i]));
                        }
                    }

                }
                else
                {
                    mail.Bcc.Add(new MailAddress(BCC));
                }
            }
            if (attach.Length > 0)
            {
                if (attach.Length > 1)
                {
                    for (int i = 0; i < attach.Length; i++)
                    {
                        string duongdan = attach[i];
                        mail.Attachments.Add(new Attachment(duongdan));
                    }
                }
                else
                {
                    string duongdan1 = attach[0];
                    mail.Attachments.Add(new Attachment(duongdan1));
                }
            }
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
            }
        }
    }
}