using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace DBTOOL.Helper
{
    class EmailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">收件人列表</param>
        /// <param name="msg">邮件正文内容</param>
        /// <param name="title">邮件标题</param>
        public static void sendMail(String email, String msg, String title)
        {

            MailMessage message = new MailMessage();
            //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
            MailAddress fromAddr = new MailAddress("weiyimings@163.com");
            message.From = fromAddr;
            //设置收件人,可添加多个,添加方法与下面的一样
            message.To.Add(email);
            //设置抄送人
            //message.CC.Add("izhaofu@163.com");
            //设置邮件标题
            message.Subject = title;
            //设置邮件内容
            message.Body = msg;
            //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看,下面是QQ的
            SmtpClient client = new SmtpClient("smtp.163.com", 25);
            //设置发送人的邮箱账号和密码
            client.Credentials = new NetworkCredential("weiyimings@163.com", "teamosiecle1");
            //启用ssl,也就是安全发送
            client.EnableSsl = true;
            //发送邮件
            client.Send(message);
        }
    }
}
