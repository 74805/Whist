using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using Common;
using System.IO;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace Client
{
    public partial class Form1 : Form
    {
        private ICommon myICommon;
        private int clientid;
        private List<Label> cards = new List<Label>();
        private List<Label> othercards1 = new List<Label>();
        private List<Label> othercards2 = new List<Label>();
        private List<Label> othercards3 = new List<Label>();
        private int[] betnum = new int[4];
        private string[] betshape = new string[4];
        private Button[] cnum = new Button[9];
        private Button[] csha = new Button[4];
        private string trump;
        private int firstid = -1;
        private string[] pnames = new string[4];
        private TextBox pnamesasking;
        private Button[] firstturn = new Button[4];
        private Button choosetrump = new Button();
        private Button circle = new Button();
        private Label txt = new Label();
        private Label[] thisroundlabels = new Label[4];
        private Timer timer = new Timer();
        private Card[] thisroundcards;
        private int winnerid;
        private Button nextround = new Button();
        private Label winner = new Label();
        private Label[] names = new Label[4];
        private Label[] take = new Label[4];
        private TextBox Ip = new TextBox();
        public Form1()
        {
            //this.TopMost = true;
           // this.WindowState = FormWindowState.Maximized;

            Ip.Location = new Point(1000, 500);
            Ip.Text = "Enter ip";
            Controls.Add(Ip);

            Button btn1 = new Button();
            btn1.Location = new Point(1000, 520);
            btn1.Text = "Submit";
            btn1.Click += GotIp;
            Controls.Add(btn1);

            HttpChannel channel = new HttpChannel();
            ChannelServices.RegisterChannel(channel, false);



        }
        public void GotIp(object sender,EventArgs args)
        {//לאחר שהזנת כתובת, הפעולה הזאת מבקשת שם.
            myICommon = (ICommon)Activator.GetObject(
                typeof(ICommon),

                "http://" + Ip.Text + ":1234/_Server_");

            Controls.Remove((Button)sender);
            Controls.Remove(Ip);
            Ip = null;
            pnamesasking = new TextBox();

            pnamesasking.Location = new Point(1000, 500);
            pnamesasking.Text = "Enter your name";

            Controls.Add(pnamesasking);
            Button btn = new Button();
            btn.Location = new Point(1000, 520);
            btn.Text = "Submit";
            btn.Click += StartGame;
            Controls.Add(btn);


            timer.Tick += Timer1;
            timer.Interval = 500;

        }

        public void Timer1(object sender, EventArgs args)
        {//(...המשתמש מגיע לפעולה הזאת כל 50 מילישניות על מנת להתעדכן במצב המשחק (אם מתחיל המשחק, אם נגמר הסיבוב
            thisroundcards = myICommon.ThisRoundCards();
            if (myICommon.GetRemoveLabels())
            {
                for (int i = 0; i < 4; i++)
                {
                    if (thisroundlabels[i] != null)
                    {
                        Controls.Remove(thisroundlabels[i]);
                        thisroundlabels[i] = null;
                    }
                }
                Controls.Remove(nextround);
                Controls.Remove(nextround);
                Controls.Remove(winner);
            }
            int counter = 0;

            pnames = myICommon.GetNames();
            for (int i = 0; i < 4; i++)
            {
                if (pnames[i] != null)
                {
                    counter++;
                }
            }
            if (counter == 4)
            {
                if (names[3]==null)
                {
                    ShowNames();
                }
                {
                    turn2();
                    int thisroundplays = 0;
                    int lastcheckedplays = 0;


                    for (int i = 0; i < 4; i++)
                    {
                        if (thisroundcards[i] != null)
                        {
                            thisroundplays++;
                        }
                        if (thisroundlabels[i] != null)
                        {
                            lastcheckedplays++;
                        }
                    }
                    if (thisroundplays > lastcheckedplays)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (thisroundcards[i] != null && thisroundlabels[i] == null)
                            {
                                thisroundlabels[i] = new Label();
                                if (thisroundlabels[i].Image == null)
                                {
                                    Image img = Image.FromFile("..\\..\\..\\PNG\\" + thisroundcards[i].GetNum().ToString() + thisroundcards[i].GetShape().ToUpper()[0] + ".png");
                                    img = Resize(img, 86, 132);
                                    thisroundlabels[i].Size = img.Size;
                                    thisroundlabels[i].Image = img;
                                    thisroundlabels[i].Tag = thisroundcards[i];

                                }
                            }
                        }

                        if (clientid == 0)
                        {
                            if (thisroundcards[1] != null)
                            {
                                thisroundlabels[1].Location = new Point(1400, 450);

                            }

                            if (thisroundcards[2] != null)
                            {
                                thisroundlabels[2].Location = new Point(915, 200);

                            }

                            if (thisroundcards[3] != null)
                            {
                                thisroundlabels[3].Location = new Point(500, 450);
                            }
                        }
                        else
                        {

                            if (clientid == 1)
                            {
                                if (thisroundcards[0] != null)
                                {

                                    thisroundlabels[0].Location = new Point(500, 450);

                                }

                                if (thisroundcards[2] != null)
                                {
                                    thisroundlabels[2].Location = new Point(1400, 450);

                                }

                                if (thisroundcards[3] != null)
                                {
                                    thisroundlabels[3].Location = new Point(915, 200);

                                }
                            }
                            else
                            {

                                if (clientid == 2)
                                {
                                    if (thisroundcards[0] != null)
                                    {

                                        thisroundlabels[0].Location = new Point(915, 200);

                                    }

                                    if (thisroundcards[1] != null)
                                    {
                                        thisroundlabels[1].Location = new Point(500, 450);

                                    }

                                    if (thisroundcards[3] != null)
                                    {
                                        thisroundlabels[3].Location = new Point(1400, 450);

                                    }
                                }
                                else
                                {


                                    
                                        if (thisroundcards[0] != null)
                                        {
                                            thisroundlabels[0].Location = new Point(1400, 450);


                                        }

                                        if (thisroundcards[1] != null)
                                        {
                                            thisroundlabels[1].Location = new Point(915, 200);

                                        }

                                        if (thisroundcards[2] != null)
                                        {
                                            thisroundlabels[2].Location = new Point(500, 450);

                                        }
                                     
                                }
                            }
                        }
                    }
                    if (thisroundplays == 4)
                    {
                        turn2();
                        CheckWinner();
                    }
                }
            }
        }
        public void ShowNames()
        {//לאחר שכל השחקנים התחברו, הפעולה מראה את השם של כל שחקן על המסך ואת מספר הסיבובים שבהן ניצח
            for (int i = 0; i < 4; i++)
            {
                names[i] = new Label();
                names[i].Text = pnames[i];

                take[i] = new Label();
                take[i].Text = 0.ToString();
                
            }
            names[clientid].Location = new Point(950, 950);
            take[clientid].Location = new Point(750, 950);
            if (clientid != 3)
            {
                names[clientid + 1].Location = new Point(1800, 475);
                take[clientid+1].Location = new Point(1800, 375);
            }
            else
            {
                names[0].Location = new Point(1800, 475);
                take[0].Location = new Point(1800, 375);
            }
            if (clientid == 0 || clientid == 1)
            {
                names[clientid + 2].Location = new Point(900, 10);
                take[clientid + 2].Location = new Point(700, 10);
            }
            else
            {
                if (clientid == 2)
                {
                    names[0].Location=  new Point(900, 10);
                    take[0].Location = new Point(700, 10);
                }
                else
                {
                    names[1].Location = new Point(900, 10);
                    take[1].Location = new Point(700, 10);
                }
            }
            if (clientid == 0)
            {
                names[3].Location = new Point(80, 475);
                take[3].Location = new Point(80, 375);
            }
            else
            {
                if (clientid == 1)
                {
                    names[0].Location = new Point(80, 475);
                    take[0].Location = new Point(80, 375);
                }
                else
                {
                    if (clientid == 2)
                    {
                        names[1].Location = new Point(80, 475);
                        take[1].Location = new Point(80, 375);
                    }
                    else
                    {
                        names[2].Location = new Point(80, 475);
                        take[2].Location = new Point(80, 375);
                    }
                }
            }
            for (int i = 0; i < 4; i++)
            {
                Controls.Add(take[i]);
                Controls.Add(names[i]);
            }
        }
        public void CheckWinner()
        {//כאשר נגמר סיבוב, הפעולה מראה לשחקנים מי ניצח ונותנת למנצח לבחור מתי לעבור לסיבוב הבא
            Card[] usedcards = myICommon.UsedCards();
            winnerid = myICommon.GetWinnerId();
            if ((int)(take[0].Text[0]) + (int)take[1].Text[0] + (int)take[2].Text[0] + (int)take[3].Text[0] -48*4< usedcards.Length)
            {
                take[winnerid].Text = ((int)take[winnerid].Text[0] -47).ToString();
            }
            if (winnerid == clientid)
            {
                nextround.Text = "המשך לסיבוב הבא";
                nextround.Location = new Point(900, 400);
                nextround.Size = new Size(100, 100);
                nextround.Click += NextRound;
                Controls.Add(nextround);
            }

            winner.Text = pnames[winnerid] + " !ניצח בסיבוב";
            winner.Location = new Point(900, 500);
            Controls.Add(winner);
        }
        public void NextRound(object sender, EventArgs args)
        {
            myICommon.WinnerId(winnerid);
        }

        public void StartGame(object sender, EventArgs args)
        {//לאחר שכל השחקנים הזינו את שמם, הפעולה מתחילה את המשחק ונותנת לשחקן האחרון שנכנס לקבוע מה הקלף השליט ואיזה שחקן יתחיל
            
            timer.Start();
            
            clientid = myICommon.Id();
            myICommon.Name(clientid, pnamesasking.Text);
            pnames = myICommon.GetNames();
            
            
            Controls.Remove((Button)sender);
            Controls.Remove(pnamesasking);
            Createcards();
            if (clientid==3)
            {
                txt.Text = "או";
                choosetrump.Text = "בחרו את השליט";
                circle.Text = "ערכו סבב בין השחקנים";
                choosetrump.Size = new Size(125, 25);
                circle.Size = new Size(200, 25);
                choosetrump.Location = new Point(1000, 500);
                txt.Location = new Point(965, 505);
                circle.Location = new Point(750, 500);
                choosetrump.Click += GetTrump;
                // circle.Click += Declare;
                Controls.Add(choosetrump);
                Controls.Add(circle);
                Controls.Add(txt);
            }
        }

        public void SetTrumpCard(object sender, EventArgs args)
        {//הפעולה מקבלת את הקלף השליט ומעבירה אותו אל השרת
            Button btn = (Button)sender;
            trump = btn.Tag.ToString();
            myICommon.SetTrump(trump);
            for (int i = 0; i < 4; i++)
            {
                Controls.Remove(csha[i]);
                firstturn[i] = new Button();
                firstturn[i].Location = new Point(770 + 100 * i, 700);
                firstturn[i].Size = csha[0].Size;
                firstturn[i].Text = pnames[i];
                firstturn[i].Click += SetFirstPlayer;
                Controls.Add(firstturn[i]);
            }
        }
        public void SetFirstPlayer(object sender, EventArgs args)
        {//הפעולה מבקשת את השחקן שיתחיל
            Button btn = (Button)sender;
            string name = btn.Text;
            for (int i = 0; i < 4; i++)
            {
                Controls.Remove(firstturn[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                if (pnames[i] == name)
                {
                   
                    firstid = i;
                    myICommon.WinnerId(firstid);
                    myICommon.SetFirsttPlayer(firstid);
                    break;
                }
            }
            
        }
        public void GetTrump(object sender, EventArgs args)
        {//הפעולה מבקשת את הקלף השליט
            Controls.Remove(txt);
            Controls.Remove(choosetrump);
            Controls.Remove(circle);
            for (int i = 0; i < 4; i++)
            {
                csha[i] = new Button();
            }
            csha[0].Tag = "spades";
            csha[1].Tag = "hearts";
            csha[2].Tag = "diamonds";
            csha[3].Tag = "clubs";
            for (int i = 0; i < 4; i++)
            {

                Image img = Image.FromFile("..\\..\\..\\PNG\\" + i.ToString() + ".png");
                img = Resize(img, 70, 70);
                csha[i].Image = img;
                csha[i].Location = new Point(770 + 100 * i, 700);
                csha[i].Size = img.Size;
                csha[i].Click += SetTrumpCard;
                Controls.Remove(cnum[i]);
                Controls.Add(csha[i]);

            }

        }
        /* public void Declare(object sender, EventArgs args)
         {

             for (int i = 0; i < 9; i++)
             {
                 cnum[i] = new Button();
                 cnum[i].Text = (i + 5).ToString();
                 cnum[i].Location = new Point(518 + 100 * i, 700);
                 string str = (i + 5).ToString();
                 cnum[i].Tag = i + 5;
                 cnum[i].Size = new Size(50, 50);
                 cnum[i].Click += SetBetNumber;
                 Controls.Add(cnum[i]);
             }

         }
         /*public void SetBetNumber(object sender, EventArgs args)
         {

             if (myICommon.IsCurrneTurn(clientid))
             {
                 Button btn = (Button)sender;
                 betnum[clientid] = (int)btn.Tag;
                 CardShape();

             }

         }*/

        public void turn2()
        {//הפעולה 
            for (int i = 0; i < 4; i++)
            {
                if (thisroundlabels[i] != null && thisroundlabels[i].Location.X != 0 && !Controls.Contains(thisroundlabels[i]))
                {
                    Controls.Add(thisroundlabels[i]);
                }
            }
            if (myICommon.IsCurrneTurn(clientid))
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    if (cards[i] != null)
                    {
                        cards[i].Click += CardClick;
                    }
                }

            }
        }

        public Image Resize(Image image, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            Graphics grp = Graphics.FromImage(bmp);
            grp.DrawImage(image, 0, 0, w, h);
            grp.Dispose();

            return bmp;
        }
        public void Createcards()
        {
            Card[] cards = myICommon.GetCards(clientid);
            int spa = 0, dia = 0, hea = 0;

            List<Card> spad = new List<Card>();
            List<Card> hear = new List<Card>();
            List<Card> diam = new List<Card>();
            List<Card> clu = new List<Card>();

            Card[] cards1 = new Card[13];

            for (int i = 0; i < 13; i++)
            {
                if (cards[i].GetShape() == "spades")
                {
                    spad.Add(cards[i]);
                    spa++;
                }
                if (cards[i].GetShape() == "hearts")
                {
                    hear.Add(cards[i]);
                    hea++;
                }
                if (cards[i].GetShape() == "diamonds")
                {
                    diam.Add(cards[i]);
                    dia++;
                }
                if (cards[i].GetShape() == "clubs")
                {
                    clu.Add(cards[i]);
                }
            }
            for (int i = 0; i < spa; i++)
            {
                for (int j = i + 1; j < spa; j++)
                {
                    if (spad[i].GetNum() < spad[j].GetNum())
                    {
                        Card card = new Card(spad[j]);
                        spad[j] = new Card(spad[i]);
                        spad[i] = card;

                    }
                }
                cards1[i] = spad[i];
            }

            for (int i = 0; i < hea; i++)
            {
                for (int j = i + 1; j < hea; j++)
                {
                    if (hear[i].GetNum() < hear[j].GetNum())
                    {
                        Card card = new Card(hear[j]);
                        hear[j] = new Card(hear[i]);
                        hear[i] = card;

                    }
                }
                cards1[i + spa] = hear[i];
            }
            for (int i = 0; i < dia; i++)
            {
                for (int j = i + 1; j < dia; j++)
                {
                    if (diam[i].GetNum() < diam[j].GetNum())
                    {
                        Card card = new Card(diam[j]);
                        diam[j] = new Card(diam[i]);
                        diam[i] = card;

                    }
                }
                cards1[i + spa + hea] = diam[i];
            }

            for (int i = 0; i < 13 - spa - hea - dia; i++)
            {
                for (int j = i + 1; j < 13 - spa - hea - dia; j++)
                {
                    if (clu[i].GetNum() < clu[j].GetNum())
                    {
                        Card card = new Card(clu[j]);
                        clu[j] = new Card(clu[i]);
                        clu[i] = card;

                    }
                }
                cards1[i + spa + hea + dia] = clu[i];
            }

            for (int i = 0; i < 13; i++)
            {
                Card card = cards1[i];
                Label label = new Label();
                string str = card.GetNum().ToString() + card.GetShape().ToUpper()[0];
                Image img = Image.FromFile("..\\..\\..\\PNG\\" + str + ".png");
                img = Resize(img, 86, 132);
                label.Image = img;
                label.Location = new Point(375 + i * 90, 800);
                label.Size = img.Size;
                label.Tag = card;
                this.cards.Add(label);
                Controls.Add(label);

                Label label1 = new Label();
                Image img1 = Image.FromFile("..\\..\\..\\PNG\\gray_back.png");
                img1 = Resize(img1, 132, 86);
                label1.Image = img1;
                label1.Size = img1.Size;
                label1.Location = new Point(1600, 200 + i * 40);
                Controls.Add(label1);
                othercards1.Add(label1);

                Label label2 = new Label();
                Image img2 = Image.FromFile("..\\..\\..\\PNG\\green_back.png");
                img2 = Resize(img2, 132, 86);
                label2.Image = img2;
                label2.Size = img2.Size;
                label2.Location = new Point(190, 200 + i * 40);
                Controls.Add(label2);
                othercards2.Add(label2);

                Label label3 = new Label();
                Image img3 = Image.FromFile("..\\..\\..\\PNG\\blue_back.png");
                img3 = Resize(img3, 86, 132);
                label3.Image = img3;
                label3.Size = img3.Size;
                label3.Location = new Point(665 + i * 40, 60);
                Controls.Add(label3);
                othercards3.Add(label3);
            }
        }
        public void CardClick(object sender, EventArgs args)
        {
            Label label = (Label)sender;

            if (myICommon.IsCurrneTurn(clientid))
            {
                if (myICommon.GetLastWinner() == clientid || !myICommon.DoesHaveShape(clientid))
                {
                    cards.Remove(label);
                    myICommon.GetLastCcard((Card)label.Tag, clientid);
                    int p = label.Location.X;
                    label.Location = new Point(375 + 6 * 90, 600);
                    thisroundlabels[clientid] = label;
                    this.cards.Remove(label);

                    for (int i = 0; i < this.cards.Count; i++)
                    {
                        if (this.cards[i].Location.X > p)
                        {
                            this.cards[i].Location = new Point(this.cards[i].Location.X - 45, 800);
                        }
                        else
                        {
                            this.cards[i].Location = new Point(this.cards[i].Location.X + 45, 800);
                        }
                    }
                }
                else
                {
                    Card cardtemp = (Card)label.Tag;
                    if (myICommon.IsCorrectShape(cardtemp))
                    {
                        cards.Remove(label);
                        myICommon.GetLastCcard((Card)label.Tag, clientid);
                        int p = label.Location.X;
                        label.Location = new Point(375 + 6 * 90, 600);
                        thisroundlabels[clientid] = label;
                        this.cards.Remove(label);

                        for (int i = 0; i < this.cards.Count; i++)
                        {
                            if (this.cards[i].Location.X > p)
                            {
                                this.cards[i].Location = new Point(this.cards[i].Location.X - 45, 800);
                            }
                            else
                            {
                                this.cards[i].Location = new Point(this.cards[i].Location.X + 45, 800);
                            }

                        }
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
