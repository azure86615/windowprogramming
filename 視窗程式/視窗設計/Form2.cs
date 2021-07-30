using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace 視窗設計
{
    public partial class Form2 : Form
    {
        static Graphics g;
        //double ox = 400, oy = 150;      //母球球心
        //double redx = 300, redy = 300;  //紅球球心

        double fr;                      //摩擦力
        double Ax, Ay, Bx, By;          //A為竿子頭、B為竿子尾的座標
        static int r,r2,r3;             //半徑、直徑、半半徑

        Point p1,p2;                    //竿子頭尾兩點 (畫線用)
        int ex, ey;                     //滑鼠作標
        static int num = 10;

        class vector {
            public double x = 0;        //前進方向X向量
            public double y = 0;        //前進方向Y向量
            public double length;       //前進方向向量長度 (速度)
            public double direct;       //前進方向角度
            public void setlength() {
                length = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            }
            public void setdirect() {
                direct = Math.Atan2(y , x);
            }
            public void setx() {
                x = length * Math.Cos(direct);
            }
            public void sety() {
                y = length * Math.Sin(direct);
            }
        }
        vector line = new vector();
        vector replace = new vector();

        class BALL {
            public double x,y;
            public vector front = new vector();
            public vector front_horizen = new vector();
            public vector front_vertical = new vector();
            Color c;
            SolidBrush br;

            public BALL(int bx, int by, Color cc) {
                x = bx;
                y = by;
                c = cc;
                br = new SolidBrush(cc);
            }
            public void draw() {
                g.FillEllipse(br, (int)(x - r), (int)(y - r), r2, r2);
            }
        }
        BALL[] ball = new BALL[num];
        BALL oball = new BALL(400, 150, Color.White);

        public Form2()
        {
            InitializeComponent();
            g = panel1.CreateGraphics();
            for (int i = 0; i < num; i++) {
                ball[i] = new BALL(i * (r2 + 50) + 30, 300, Color.FromArgb((i + 1) * 50 % 256, (i + 1) * 60 % 256, (i + 1) * 100 % 256));
            }
        }

        //回首頁
        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Owner = this;
            this.Hide();
            f1.Show();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label1.Text = ((Form1)Owner).textBox1.Text + "歡迎您試玩！";
            label2.Text = ("初速");
            label3.Text = ("摩擦力");
            
            r = 15;
            r2 = r * 2;
            r3 = r / 2;

            Ax = oball.x + r * Math.Cos(oball.front.direct * Math.PI / 180);
            Ay = oball.y + r * Math.Sin(oball.front.direct * Math.PI / 180);
            Bx = oball.x + 8 * r * Math.Cos(oball.front.direct * Math.PI / 180);
            By = oball.y + 8 * r * Math.Sin(oball.front.direct * Math.PI / 180);
            p1 = new Point((int)Ax, (int)Ay);
            p2 = new Point((int)Bx, (int)By);

            fr = hScrollBar2.Value / 50.0;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {            
            oball.draw();
            for (int i = 0; i < num; i++)
                ball[i].draw();
            g.DrawLine(Pens.Orange, p1, p2);
            g.FillEllipse(Brushes.Gray,ex-r3,ey-r3,r,r);
        }

        //滑鼠點桌面
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (timer1.Enabled == false)
            {
                oball.front.direct = Math.Atan2(e.Y - oball.y, e.X - oball.x);
                Ax = oball.x + r * Math.Cos(oball.front.direct - Math.PI);
                Ay = oball.y + r * Math.Sin(oball.front.direct - Math.PI);
                Bx = oball.x + 8 * r * Math.Cos(oball.front.direct - Math.PI);
                By = oball.y + 8 * r * Math.Sin(oball.front.direct - Math.PI);
                p1 = new Point((int)Ax, (int)Ay);
                p2 = new Point((int)Bx, (int)By);

                oball.front.length = hScrollBar1.Value;
            }
            ex =(int)e.X ;
            ey =(int)e.Y ;
            panel1.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            move();
            rebound();
            pullback();
            panel1.Refresh();
        }

        //點擊hit
        private void button2_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == false)
            { 
                timer1.Enabled = true;
            }
        }

        //點擊stop
        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        //移動
        private void move()
        {
            oball.front.setx();
            oball.front.sety();
            oball.x += oball.front.x;
            oball.y += oball.front.y;
            for (int i = 0; i < num; i++)
            {
                ball[i].front.setx();
                ball[i].front.sety();
                ball[i].x += ball[i].front.x;
                ball[i].y += ball[i].front.y;

                if (ball[i].front.length > 0)
                    ball[i].front.length -= fr;
                else
                    ball[i].front.length = 0;

                for (int j = 0; j < num; j++)
                {
                    if (j == i) ;
                    else if (Math.Pow(ball[i].x - ball[j].x, 2) + Math.Pow(ball[i].y - ball[j].y, 2) <= (r2 * r2))
                    {
                        collism(ball[j], ball[i]);                        
                    }
                }
            }
            if (oball.front.length > 0)
                oball.front.length -= fr;
            else
                oball.front.length = 0;

            if (oball.front.length == 0)
            {
                int ct = 0;
                for (int i = 0; i < num; i++)
                {
                    if (ball[i].front.length == 0) ct++;
                }
                if (ct == 10) timer1.Enabled = false;
            }
            for (int i = 0; i < num; i++)
            {
                if (Math.Pow(ball[i].x - oball.x, 2) + Math.Pow(ball[i].y - oball.y, 2) <= (r2 * r2))
                {
                    collism(ball[i], oball);
                    //checkBox1.Checked = false;
                }
            }
        }
        //碰撞
        private void collism(BALL a, BALL b)
        {   //母球的投影、算分量
            line.x = b.x - a.x;
            line.y = b.y - a.y;
            line.setlength();
            double co = (a.front.x * line.x + a.front.y * line.y) / (Math.Pow(line.length, 2));
            a.front_horizen.x = co * line.x;
            a.front_horizen.y = co * line.y;
            a.front_vertical.x = a.front.x - a.front_horizen.x;
            a.front_vertical.y = a.front.y - a.front_horizen.y;

            //母球外的投影、算分量
            line.x = a.x - b.x;
            line.y = a.y - b.y;
            line.setlength();
            co = (b.front.x * line.x + b.front.y * line.y) / (Math.Pow(line.length, 2));
            b.front_horizen.x = co * line.x;
            b.front_horizen.y = co * line.y;
            b.front_vertical.x = b.front.x - b.front_horizen.x;
            b.front_vertical.y = b.front.y - b.front_horizen.y;

            //重合成新向量 (垂直分量不變，水平分量交換)
            a.front.x = b.front_horizen.x + a.front_vertical.x;
            a.front.y = b.front_horizen.y + a.front_vertical.y;
            b.front.x = a.front_horizen.x + b.front_vertical.x;
            b.front.y = a.front_horizen.y + b.front_vertical.y;
            
            //設定新前進方向以及速度
            a.front.setdirect();
            a.front.setlength();
            b.front.setdirect();
            b.front.setlength();
        }

        //反彈
        private void rebound() {
            if (oball.x < r || oball.x > panel1.Width- r) {
                oball.front.direct = Math.PI - oball.front.direct;
            }
            if (oball.y < r || oball.y > panel1.Height-r) {
                oball.front.direct = -oball.front.direct;
            }

            for (int i = 0; i < num; i++)
            {
                if (ball[i].x < r || ball[i].x > panel1.Width - r)
                {
                    ball[i].front.direct = Math.PI - ball[i].front.direct;
                }
                if (ball[i].y < r || ball[i].y > panel1.Height - r)
                {
                    ball[i].front.direct = -ball[i].front.direct;
                }
            }
        }

        //防止卡在邊緣，拉回範圍內
        private void pullback()         
        {
            if (oball.x > panel1.Width - r)
                oball.x = panel1.Width - r;
            if (oball.y > panel1.Height -r)
                oball.y = panel1.Height -r;
            if (oball.x < r)
                oball.x = r;
            if (oball.y < r)
                oball.y = r;

            for (int i = 0; i < num; i++)
            {
                if (ball[i].x > panel1.Width - r)
                    ball[i].x = panel1.Width - r;
                if (ball[i].y > panel1.Height - r)
                    ball[i].y = panel1.Height - r;
                if (ball[i].x < r)
                    ball[i].x = r;
                if (ball[i].y < r)
                    ball[i].y = r;
            }
        }

        //摩擦力拉桿改變
        private void hScrollBar2_ValueChanged(object sender, EventArgs e) 
        {
            fr = hScrollBar2.Value / 50.0;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
                timer1.Enabled = false;
            else
                timer1.Enabled = true;
        }
    }
}
