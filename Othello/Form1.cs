using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Othello
{
    public partial class Form1 : Form
    {
        Bitmap[] img = new Bitmap[4];
        static int[,] bo = new int[10, 10];
        static int cs; //Current Stone
        static string[] color = {"","黒","白" };
        static int[] cntStn;

        public Form1()
        {
            InitializeComponent();
            panel1.Left = 40;
            panel1.Top = 40;
            panel1.Width = 321;
            panel1.Height = 321;
            Width = 600;
            Height = 450;
            img[0] = Properties.Resources.none;
            img[1] = Properties.Resources.black;
            img[2] = Properties.Resources.white;
            img[3] = Properties.Resources.set;
            label1.Text = "新規ゲームをクリックしてください";
            label2.Text = "";
            label3.Text = "黒：";
            label4.Text = "白：";
            panel1.Enabled = false;

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i < 9; i++)
            {
                g.DrawLine(Pens.Black, 0, 40 * i, 320, 40 * i);
                g.DrawLine(Pens.Black, 40 * i, 0, 40 * i, 320);
            }
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    g.DrawImage(img[getBo(i,j)], (i - 1) * 40 + 4, (j - 1) * 40 + 4, 32, 32);//画像描写
                }
            }
        }
        static int getBo(int x, int y)
        {
            return bo[x, y];
        }
        static void setBo(int s, int x, int y)
        {
            bo[x, y] = s;
        }
        private void point(object sender, EventArgs e)
        {
            Point getXY = PointToClient(Control.MousePosition);
            int x = (getXY.X) / 40;
            int y = (getXY.Y) / 40;
            //label5.Text = ("x=" + getXY.X.ToString() + " y=" + getXY.Y.ToString());
            input(cs, x, y);
            Refresh();
        }
        void plusCount()
        {
            cs = cs % 2 + 1;
        }
        void countStone()
        {
            cntStn = new int[4];
            for(int i = 0;i< 4; i++)
            {
                cntStn[i] = 0;
            }
            for (int i = 1; i < 9;i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    cntStn[bo[i,j]]++;
                }

            }
        }
        void input(int s, int x, int y)
        {
            label1.Text = ("");
            label2.Text = ("");
            if (getBo(x,y)==3)
            {
                setBo(s,x,y);
                turn(s, x, y);
                plusCount();//相手に渡す
                setCand(cs);
                countStone();
                if (cntStn[3] == 0)//相手の置ける場所が0の場合
                {
                    plusCount();//味方に渡す
                    setCand(cs);
                    countStone();
                    if(cntStn[3] == 0)//味方の置ける場所が0の場合
                    {
                        exitGame();//ゲームを終了
                    }
                    else
                    {
                        label1.Text = ("置ける場所がないのでパスします");//テキスト表示
                        label2.Text = (color[cs] + "の番です");
                    }
                }
                else
                {
                    label1.Text = (color[cs] + "の番です");
                }
            }else
            {
                label1.Text = ("その場所には置けません");
                label2.Text = (color[cs] + "の番です");
            }
            textCount();
        }
        void setCand(int s)
        {
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    if (getBo(i,j) == 0 || getBo(i, j) == 3)
                    {
                        if (srch(s,i,j))
                        {
                            setBo(3,i,j);
                        }else
                        {
                            setBo(0,i,j);
                        }
                        
                    }
                }
            }
        }
        bool srch(int s, int x, int y)
        {
            bool isBreak = false;
            bool judge = false;
            int cc; //current coordinate
            for (int v = -1; v < 2; v++)
            {
                for (int h = -1; h < 2; h++)
                {
                    if (h == 0 && v == 0)
                        continue;
                    for (int i = 1; i < 9; i++)
                    {
                        cc = bo[x + h * i, y + v * i];
                        if (cc == 0 || cc == 3)
                        {//kuurann
                            isBreak = true;
                        }
                        else if (cc == s % 2 + 1)
                        {//enemy no iro
                            continue;
                        }
                        else if (cc == s)
                        {//mikata no iro
                            if (i > 1)
                            {
                                judge = true;
                                isBreak = true;
                            }
                            else
                            {
                                isBreak = true;
                            }
                        }
                        if (isBreak)
                            break;
                    }
                    if (judge)
                        break;
                }
                if (judge)
                    break;
            }
            return judge;
        }
        void turn(int s, int x, int y)
        {
            int cc; //current coordinate
            for (int v = -1; v < 2; v++)
            {
                for (int h = -1; h < 2; h++)
                {
                    if (v == 0 && h == 0)
                        continue;
                    for (int i = 1; i < 8; i++)
                    {
                        cc = bo[x + h * i, y + v * i];
                        if (cc == s && i > 1)
                        {
                            for (int j = 0; j < i; j++)
                            {
                                setBo(s, x + h * j, y + v * j);
                            }
                        }
                        else if (cc == 0 || cc == 3)
                        {
                            break;
                        }
                    }
                }
            }
        }
        void exitGame()
        {
            panel1.Enabled = false;
            if (cntStn[1]>cntStn[2])
            {
                label1.Text = "黒が勝ちました";
            }
            else if (cntStn[2]>cntStn[1])
            {
                label1.Text = "白が勝ちました";
            }else
            {
                label1.Text = "引き分けです";
            }

        }
        void textCount()
        {
            label3.Text = "黒：" + cntStn[1].ToString() ;
            label4.Text = "白：" + cntStn[2].ToString();
        }

        private void newGame(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    setBo(0, i, j);
                }
            }
            cs = 1;
            bo[4, 4] = 1;
            bo[5, 5] = 1;
            bo[4, 5] = 2;
            bo[5, 4] = 2;
            setCand(cs);
            countStone();
            textCount();
            label1.Text = (color[cs] + "の番です");
            label2.Text = ("");
            panel1.Enabled = true;
            panel1.Refresh();
        }

        private void ExitApp(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
    //class Player
    //{
    //    int color;
    //    int cntStn;

    //}
}
