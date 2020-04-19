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
        private int clientid = -1;
        private List<Label> cards;
        private List<Label> othercards1;
        private List<Label> othercards2;
        private List<Label> othercards3;
        private Button[] cnum;
        private Button[] csha;
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
        private string declarenum;
        private Button pass;
        private int currentturn = -1;
        private Label turnname = new Label();
        private Label[] declares = new Label[4];
        private string thisdeclare;
        private int frischcounter = 0;
        private List<Card> frischcards = new List<Card>();
        private Card[,] frischpcards;
        private bool[,] alreadyincontrols = new bool[4, 3];
        private Label[,] frischlabels = new Label[4, 3];
        private bool gotnames;
        private Label itsfrisch;
        public Form1()
        {
            //this.TopMost = true;
            //this.WindowState = FormWindowState.Maximized;
            //   BackColor = Color.Green;
            Ip.Location = new Point(700, 300);
            Ip.Text = "localhost";// "213.57.202.58"; //"Enter ip";
            Ip.Click += RemoveText;

            Controls.Add(Ip);

            Button btn1 = new Button();
            btn1.BackColor = Color.White;
            btn1.Location = new Point(700, 320);
            btn1.Text = "Submit";
            btn1.Click += GotIp;
            Controls.Add(btn1);

            HttpChannel channel = new HttpChannel();
            ChannelServices.RegisterChannel(channel, false);



        }
        public void RemoveText(object sender, EventArgs Args)
        {
            TextBox removetext = (TextBox)sender;
            removetext.Text = "";
        }
        public void GotIp(object sender, EventArgs args)
        {//לאחר שהזנת כתובת, הפעולה הזאת מבקשת שם.
            myICommon = (ICommon)Activator.GetObject(
                typeof(ICommon),

                "http://" + Ip.Text + ":1234/_Server_");

            Controls.Remove((Button)sender);
            Controls.Remove(Ip);
            Ip = null;
            pnamesasking = new TextBox();
            pnamesasking.Location = new Point(700, 300);
            pnamesasking.Text = "Enter your name";
            pnamesasking.Click += RemoveText;

            Controls.Add(pnamesasking);
            Button btn = new Button();
            btn.Location = new Point(700, 320);
            btn.BackColor = Color.White;
            btn.Text = "Submit";
            btn.Click += StartGame;
            Controls.Add(btn);


            timer.Tick += Timer1;
            timer.Interval = 50;

        }

        public void Timer1(object sender, EventArgs args)
        {//(...המשתמש מגיע לפעולה הזאת כל 50 מילישניות על מנת להתעדכן במצב המשחק (אם מתחיל המשחק, אם נגמר הסיבוב

            if (gotnames || myICommon.GetNames())
            {
                if ((names[3] == null || names[2] == null || names[3].Text == "" || names[2].Text == ""))
                {
                    gotnames = true;
                    ShowNames();
                }
                else
                {
                    thisroundcards = myICommon.ThisRoundCards();
                    if (myICommon.GetRemoveCards(clientid))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (thisroundlabels[i].Image != null)
                            {
                                Controls.Remove(thisroundlabels[i]);
                                thisroundlabels[i].Image = null;
                                thisroundcards[i] = null;
                            }
                        }
                        myICommon.SetRemoveCardsFalse(clientid);
                        if (Controls.Contains(nextround))
                        {
                            Controls.Remove(nextround);
                        }
                        Controls.Remove(winner);
                    }
                    if (myICommon.NewCard(clientid))
                    {
                        NewCard();
                    }
                    if (myICommon.NewDeclare(clientid))
                    {
                        thisdeclare = myICommon.ThisDeclare();
                        declares[(int)thisdeclare[thisdeclare.Length - 1] - 48].Text = thisdeclare.Substring(0, thisdeclare.Length - 1);
                        Controls.Add(declares[(int)thisdeclare[thisdeclare.Length - 1] - 48]);
                    }


                    if (myICommon.IsNewFrischCard(clientid))
                    {
                        ShowFrischCards();
                    }

                    if (myICommon.IsCurrneTurn(clientid) || myICommon.ThisRoundPlays())
                    {
                        turn2();
                    }
                    if (myICommon.ThisRoundPlays())
                    {
                        CheckWinner();
                    }
                    if (myICommon.GetFrisch(clientid))
                    {
                        Frisch();
                    }

                    if (declares[3] != null && declares[3].Text != "" && myICommon.IsDeclareOver(clientid))
                    {
                        declares[3].Text = "";
                        for (int i = 0; i < 4; i++)
                        {
                            Controls.Remove(declares[i]);
                        }
                    }
                    if (myICommon.RemoveFrischCard(clientid))
                    {
                        int[] takencard = myICommon.TookCard(clientid);
                        Controls.Remove(frischlabels[takencard[0], takencard[1]]);
                    }
                    
                    if (currentturn != myICommon.GetTurn() && !Controls.Contains(circle))
                    {
                        CurrentTurn();
                    }
                    if (frischcards.Count!=0&&myICommon.AreAllCardsTaken())
                    {
                        Controls.Remove(itsfrisch);
                        frischcounter = 0;
                    }
                    if (myICommon.GetNextGame(clientid))
                    {
                        currentturn = -1;
                        cards = new List<Label>();
                        othercards1 = new List<Label>();
                        othercards2 = new List<Label>();
                        othercards3 = new List<Label>();
                        cnum = new Button[9];
                        csha = new Button[4];
                        trump = null;
                        firstid = -1;
                        firstturn = new Button[4];
                        choosetrump = new Button();
                        circle = new Button();
                        txt = new Label();
                        thisroundlabels = new Label[4];
                        thisroundcards = null;
                        nextround = new Button();
                        winner = new Label();
                        take = new Label[4];
                        Ip = new TextBox();
                        declarenum = null;
                        pass = null;
                        currentturn = -1;
                        turnname = new Label();
                        declares = new Label[4];
                        thisdeclare = null;
                        frischcounter = 0;
                        frischcards = new List<Card>();
                        frischpcards = null;
                        alreadyincontrols = new bool[4, 3];
                        frischlabels = new Label[4, 3];
                        Controls.Remove(turnname);


                        myICommon.SetNextGameFalse(clientid);
                        Button send = new Button();
                        Controls.Clear();
                        StartGame(send, args);
                        ShowNames();

                        
                        
                    }

                }



            }




        }
        public void ShowFrischCards()
        {
            frischpcards = myICommon.GetFrischCards();

            for (int i = 0; i < 4; i++)
            {

                for (int j = 0; j < 3; j++)
                {
                    if (frischpcards[i, j] != null && !alreadyincontrols[i, j])
                    {
                        frischlabels[i, j].Tag = frischpcards[i, j];
                        if (i == ((clientid + 1) == 4 ? 0 : (clientid + 1)))
                        {
                            frischlabels[i, j].Click += GetFrischCard;
                        }

                        Controls.Add(frischlabels[i, j]);
                        alreadyincontrols[i, j] = true;
                    }
                }

            }
        }
        public void GetFrischCard(object sender,EventArgs args)
        {
            if (frischcards.Count == 3)
            {
                Card card = (Card)((Label)sender).Tag;
                int ind = myICommon.GetCardIndex(card, clientid);

                Label newcard = new Label();
                string str = card.GetNum().ToString() + card.GetShape().ToUpper()[0];
                Image img = Image.FromFile("..\\..\\..\\PNG\\" + str + ".png");
                img = Resize(img, 54, 83);
                newcard.Size = img.Size;
                newcard.Image = img;
                newcard.Click += CardClick;
                cards.Add(newcard);
                if (ind < cards.Count - 1)
                {
                    newcard.Location = new Point(cards[ind].Location.X, 550); ;
                }
                else
                {
                    newcard.Location=new Point(cards[ind-1].Location.X+56,550);
                }

                for (int i = ind; i < cards.Count; i++)
                {
                   
                    Label card1 = cards[i];
                    cards[i] = cards[cards.Count - 1];
                    cards[cards.Count - 1] = card1;
 
                }
                for (int i = 0; i < cards.Count; i++)
                {
                    if (i > ind)
                    {
                        cards[i].Location = new Point(cards[i].Location.X + 28, 550);
                    }
                    else
                    {
                        cards[i].Location = new Point(cards[i].Location.X - 28, 550);
                    }
                }
                
                Controls.Add(newcard);

                
            }
        }
        public void Frisch()
        {
            for (int i = 0; i < 4; i++)
            {
                Controls.Remove(declares[i]);
            }

            itsfrisch = new Label();
            itsfrisch.Location = new Point(645, 320);
            itsfrisch.Text = "פריש!";
            Controls.Add(itsfrisch);
            Controls.Remove(turnname);
        }
        public void NewCard()
        {
            for (int i = 0; i < 4; i++)
            {
                thisroundcards = myICommon.ThisRoundCards();
                if (thisroundcards[i] != null && thisroundlabels[i].Image == null)
                {
                    Image img = Image.FromFile("..\\..\\..\\PNG\\" + thisroundcards[i].GetNum().ToString() + thisroundcards[i].GetShape().ToUpper()[0] + ".png");
                    img = Resize(img, 54, 83);
                    thisroundlabels[i].Image = img;
                    thisroundlabels[i].Tag = thisroundcards[i];
                    Controls.Add(thisroundlabels[i]);
                }
            }
        }
        public void CurrentTurn()
        {
            currentturn = myICommon.GetTurn();
            if (currentturn != -1)
            {
                turnname.Text = pnames[currentturn] + " התור של";
                turnname.Size = new Size(turnname.Text.Length * 10, 20);
                turnname.Location = new Point(630, 350);
                Controls.Add(turnname);
            }
        }
        public void ShowNames()
        {//לאחר שכל השחקנים התחברו, הפעולה מראה את השם של כל שחקן על המסך ואת מספר הסיבובים שבהן ניצח
            pnames = myICommon.PlayersNames();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    frischlabels[i, j] = new Label();
                }


                names[i] = new Label();
                names[i].Text = pnames[i];

                take[i] = new Label();
                take[i].Text = 0.ToString();

            }
            names[clientid].Location = new Point(660, 640);
            take[clientid].Location = new Point(550, 640);
            if (clientid != 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    frischlabels[clientid + 1, i].Location = new Point(300, 308 + 41 * i);
                    Image img1 = Image.FromFile("..\\..\\..\\PNG\\green_back.png");
                    img1 = Resize(img1, 83, 54);
                    frischlabels[clientid + 1, i].Image = img1;
                    frischlabels[clientid + 1, i].Size = img1.Size;
                }

                names[clientid + 1].Location = new Point(80, 350);
                take[clientid + 1].Location = new Point(80, 450);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    frischlabels[0, i].Location = new Point(300, 308 + 41 * i);
                    Image img1 = Image.FromFile("..\\..\\..\\PNG\\green_back.png");
                    img1 = Resize(img1, 83, 54);
                    frischlabels[0, i].Image = img1;
                    frischlabels[0, i].Size = img1.Size;
                }
                names[0].Location = new Point(80, 350);
                take[0].Location = new Point(80, 450);
            }
            if (clientid == 0 || clientid == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    frischlabels[clientid + 2, i].Location = new Point(632 + i * 28, 150);
                    Image img1 = Image.FromFile("..\\..\\..\\PNG\\blue_back.png");
                    img1 = Resize(img1, 54, 83);
                    frischlabels[clientid + 2, i].Image = img1;
                    frischlabels[clientid + 2, i].Size = img1.Size;


                }
                names[clientid + 2].Location = new Point(660, 10);
                take[clientid + 2].Location = new Point(550, 10);
            }
            else
            {
                if (clientid == 2)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        frischlabels[0, i].Location = new Point(632 + i * 28, 150);
                        Image img1 = Image.FromFile("..\\..\\..\\PNG\\blue_back.png");
                        img1 = Resize(img1, 54, 83);
                        frischlabels[0, i].Image = img1;
                        frischlabels[0, i].Size = img1.Size;

                    }
                    names[0].Location = new Point(660, 10);
                    take[0].Location = new Point(550, 10);
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        frischlabels[1, i].Location = new Point(632 + i * 28, 150);
                        Image img1 = Image.FromFile("..\\..\\..\\PNG\\blue_back.png");
                        img1 = Resize(img1, 54, 83);
                        frischlabels[1, i].Image = img1;
                        frischlabels[1, i].Size = img1.Size;
                    }
                    names[1].Location = new Point(660, 10);
                    take[1].Location = new Point(550, 10);
                }
            }
            if (clientid == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    frischlabels[3, i].Location = new Point(1000, 308 + 41 * i);
                    Image img1 = Image.FromFile("..\\..\\..\\PNG\\gray_back.png");
                    img1 = Resize(img1, 83, 54);
                    frischlabels[3, i].Image = img1;
                    frischlabels[3, i].Size = img1.Size;
                }
                names[3].Location = new Point(1200, 350);
                take[3].Location = new Point(1200, 450);
            }
            else
            {
                if (clientid == 1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        frischlabels[0, i].Location = new Point(1000, 308 + 41 * i);
                        Image img1 = Image.FromFile("..\\..\\..\\PNG\\gray_back.png");
                        img1 = Resize(img1, 83, 54);
                        frischlabels[0, i].Image = img1;
                        frischlabels[0, i].Size = img1.Size;
                    }
                    names[0].Location = new Point(1200, 350);
                    take[0].Location = new Point(1200, 450);
                }
                else
                {
                    if (clientid == 2)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            frischlabels[1, i].Location = new Point(1000, 308 + 41 * i);
                            Image img1 = Image.FromFile("..\\..\\..\\PNG\\gray_back.png");
                            img1 = Resize(img1, 83, 54);
                            frischlabels[1, i].Image = img1;
                            frischlabels[1, i].Size = img1.Size;
                        }
                        names[1].Location = new Point(1200, 350);
                        take[1].Location = new Point(1200, 450);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            frischlabels[2, i].Location = new Point(1000, 308 + 41 * i);
                            Image img1 = Image.FromFile("..\\..\\..\\PNG\\gray_back.png");
                            img1 = Resize(img1, 83, 54);
                            frischlabels[2, i].Image = img1;
                            frischlabels[2, i].Size = img1.Size;
                        }
                        names[2].Location = new Point(1200, 350);
                        take[2].Location = new Point(1200, 450);
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
            Controls.Remove(turnname);
            Card[] usedcards = myICommon.UsedCards();
            winnerid = myICommon.GetWinnerId();

            if (myICommon.isroundover(clientid) || (int)(take[0].Text[0]) + (int)take[1].Text[0] + (int)take[2].Text[0] + (int)take[3].Text[0] - 48 * 4 < usedcards.Length)
            {
                take[winnerid].Text = ((int)take[winnerid].Text[0] - 47).ToString();
            }

            if (winnerid == clientid)
            {
                if (nextround.Location.X == 0)
                {
                    nextround.Text = "המשך לסיבוב הבא";
                    nextround.Location = new Point(645, 320);
                    nextround.Size = new Size(100, 100);
                    nextround.Click += NextRound;
                }
                Controls.Add(nextround);
            }
            if (winner.Location.X == 0)
            {
                winner.Text = pnames[winnerid] + " !ניצח בסיבוב";

                winner.Location = new Point(650, 300);
            }
            Controls.Add(winner);
        }
        public void NextRound(object sender, EventArgs args)
        {

            if ((int)(take[0].Text[0]) + (int)take[1].Text[0] + (int)take[2].Text[0] + (int)take[3].Text[0] - 48 * 4 == 13)
            {
                myICommon.SetNextGame();
            }
            else
            {
                myICommon.WinnerId(winnerid);
            }
            myICommon.SetRemoveCardsTrue();
        }
        public void StartGame(object sender, EventArgs args)
        {
            timer.Start();
            if (clientid == -1)
            {
                clientid = myICommon.Id();
            }
            myICommon.Name(clientid, pnamesasking.Text);

            Image img = Image.FromFile("..\\..\\..\\PNG\\3S.png");
            img = Resize(img, 54, 83);
            for (int i = 0; i < 4; i++)
            {
                thisroundlabels[i] = new Label();
                thisroundlabels[i].Size = img.Size;
            }
            thisroundlabels[clientid].Location = new Point(670, 450);
            if (clientid == 0)
            {
                thisroundlabels[1].Location = new Point(450, 320);
                thisroundlabels[2].Location = new Point(670, 175);
                thisroundlabels[3].Location = new Point(900, 320);


            }
            else
            {

                if (clientid == 1)
                {
                    thisroundlabels[0].Location = new Point(900, 320);
                    thisroundlabels[2].Location = new Point(450, 320);
                    thisroundlabels[3].Location = new Point(670, 175);

                }
                else
                {

                    if (clientid == 2)
                    {
                        thisroundlabels[0].Location = new Point(670, 175);
                        thisroundlabels[1].Location = new Point(900, 320);
                        thisroundlabels[3].Location = new Point(450, 320);
                    }
                    else
                    {
                        thisroundlabels[0].Location = new Point(450, 320);
                        thisroundlabels[1].Location = new Point(670, 175);
                        thisroundlabels[2].Location = new Point(900, 320);
                    }
                }
            }
            for (int i = 0; i < 4; i++)
            {
                declares[i] = new Label();
                declares[i].Location = thisroundlabels[i].Location;
            }

            Controls.Remove((Button)sender);
            Controls.Remove(pnamesasking);
            Createcards();
            if (clientid == 3)
            {
                txt.Text = "או";
                choosetrump.Text = "בחרו את השליט";
                circle.Text = "ערכו סבב בין השחקנים";
                choosetrump.Size = new Size(105, 25);
                circle.Size = new Size(145, 25);
                choosetrump.Location = new Point(740, 360);
                txt.Location = new Point(709, 365);
                circle.Location = new Point(558, 360);
                choosetrump.Click += GetTrump;
                circle.Click += SetDeclare;
                Controls.Add(choosetrump);
                Controls.Add(circle);
                Controls.Add(txt);
            }
        }

        /*  bool SocketConnected(Socket s)
          {
              bool part1 = s.Poll(1000, SelectMode.SelectRead);
              bool part2 = (s.Available == 0);
              if (part1 && part2)
                  return false;
              else
                  return true;
          }*/
        public void SetTrumpCard(object sender, EventArgs args)
        {//הפעולה מקבלת את הקלף השליט ומעבירה אותו אל השרת
            Button btn = (Button)sender;
            trump = btn.Tag.ToString();
            myICommon.SetTrump(trump);
            for (int i = 0; i < 4; i++)
            {
                Controls.Remove(csha[i]);
                if (firstturn[i] == null)
                {
                    firstturn[i] = new Button();
                    firstturn[i].Location = new Point(513 + 100 * i, 467);
                    firstturn[i].Size = csha[0].Size;
                    firstturn[i].Text = pnames[i];
                    firstturn[i].Click += SetFirstPlayer;
                }
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
            csha = new Button[4];
            if (csha[0] == null)
            {
                for (int i = 0; i < 4; i++)
                {
                    csha[i] = new Button();
                }
                csha[0].Tag = "spades";
                csha[1].Tag = "hearts";
                csha[2].Tag = "diamonds";
                csha[3].Tag = "clubs";
            }
            for (int i = 0; i < 4; i++)
            {
                Image img = Image.FromFile("..\\..\\..\\PNG\\" + i.ToString() + ".png");
                img = Resize(img, 70, 70);
                csha[i].Image = img;
                csha[i].Location = new Point(513 + 100 * i, 467);
                csha[i].Size = img.Size;
                csha[i].Click += SetTrumpCard;
                Controls.Add(csha[i]);
            }

        }
        public void SetDeclare(object sender, EventArgs args)
        {

            Controls.Remove(txt);
            Controls.Remove(choosetrump);
            Controls.Remove(circle);
            myICommon.SetDeclare();
        }
        public void Declare()
        {

            if (!myICommon.GetFrisch(clientid))
            {
                if (cnum[1] == null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        if (cnum[i]==null)
                        {
                            cnum[i] = new Button();
                            cnum[i].Text = (i + 5).ToString();
                            cnum[i].Location = new Point(390 + 70 * i, 450);
                            string str = (i + 5).ToString();
                            cnum[i].Size = new Size(50, 50);
                            cnum[i].Click += SetBetNumber;
                        }
                        Controls.Add(cnum[i]);
                    }
                    if (pass == null)
                    {
                        pass = new Button();
                        pass.Text = "Pass";
                        pass.Location = new Point(660, 375);
                        pass.Size = new Size(70, 70);
                        pass.Click += SetBetNumber;
                    }
                    Controls.Add(pass);
                }
                else
                {
                    cnum[0] = new Button();
                    cnum[0].Text = (5).ToString();
                    cnum[0].Location = new Point(390, 450);
                    string str = (5).ToString();
                    cnum[0].Size = new Size(50, 50);
                    cnum[0].Click += SetBetNumber;
                    for (int i = 0; i < 9; i++)
                    {
                        Controls.Add(cnum[i]);
                    }
                    Controls.Add(pass);
                }
            }
        }
        public void SetBetNumber(object sender, EventArgs args)
        {

            Controls.Remove(pass);
            for (int i = 0; i < 9; i++)
            {
                Controls.Remove(cnum[i]);
            }
            Button btn = (Button)sender;

            declarenum = btn.Text;
            declares[clientid].Text = declarenum.ToString();
            if (declarenum != "Pass")
            {
                if (csha == null)
                {
                    csha = new Button[4];
                }
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
                    csha[i].Location = new Point(513 + 100 * i, 467);
                    csha[i].Size = img.Size;
                    csha[i].Click += DeclareTrump;
                    Controls.Remove(cnum[i]);
                    Controls.Add(csha[i]);
                }
            }
            else
            {
                Button btn1 = new Button();
                btn1.Tag = "pass";
                DeclareTrump(btn1, args);
            }

        }
        public void DeclareTrump(object sender, EventArgs args)
        {
            if (csha != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    Controls.Remove(csha[i]);
                }
            }
            Button btn = (Button)sender;
            declares[clientid].Text += " " + btn.Text;
            myICommon.updatedeclare(clientid, declarenum, (string)btn.Tag);
            cnum[0] = null;
        }

        public void turn2()
        {//הפעולה    
            if (cnum == null)/*|| (frischcards.Count != 0 && frischcards[0] != null))
            {
                if (frischcards.Count!=0&&frischcards[0] != null)
                {
                    frischcards[0] = null;
                }*/
            { 
                cnum = new Button[9];
            }
            if (cnum[0] == null && trump == null && myICommon.GetDeclare(clientid) && frischcounter == 0)
            {
                Controls.Remove(declares[clientid]);
                Declare();
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
            Card[] cards1 = myICommon.GetCards(clientid);

            cards = new List<Label>();
            othercards1 = new List<Label>();
            othercards2 = new List<Label>();
            othercards3 = new List<Label>();

            for (int i = 0; i < 13; i++)
            {
                Card card = cards1[i];
                Label label = new Label();
                string str = card.GetNum().ToString() + card.GetShape().ToUpper()[0];
                Image img = Image.FromFile("..\\..\\..\\PNG\\" + str + ".png");
                img = Resize(img, 54, 83);
                label.Image = img;
                label.Location = new Point(335 + i * 56, 550);
                label.Size = img.Size;
                label.Tag = card;
                label.Click += CardClick;
                this.cards.Add(label);
                Controls.Add(label);

                Label label1 = new Label();
                Image img1 = Image.FromFile("..\\..\\..\\PNG\\gray_back.png");
                img1 = Resize(img1, 83, 54);
                label1.Image = img1;
                label1.Size = img1.Size;
                label1.Location = new Point(1100, 150 + i * 30);
                Controls.Add(label1);
                othercards1.Add(label1);

                Label label2 = new Label();
                Image img2 = Image.FromFile("..\\..\\..\\PNG\\green_back.png");
                img2 = Resize(img2, 83, 54);
                label2.Image = img2;
                label2.Size = img2.Size;
                label2.Location = new Point(190, 150 + i * 30);
                Controls.Add(label2);
                othercards2.Add(label2);

                Label label3 = new Label();
                Image img3 = Image.FromFile("..\\..\\..\\PNG\\blue_back.png");
                img3 = Resize(img3, 54, 83);
                label3.Image = img3;
                label3.Size = img3.Size;
                label3.Location = new Point(480 + i * 30, 60);
                Controls.Add(label3);
                othercards3.Add(label3);
            }
        }
        public void CardClick(object sender, EventArgs args)
        {
            Label label = (Label)sender;
            if (myICommon.GetFrisch(clientid))
            {
                myICommon.NewFrischCard((Card)label.Tag, clientid);
                bool alreadyinfrisch = false;
                for (int i = 0; i < frischcounter; i++)
                {
                    if (frischcards[i] != null && ((Card)label.Tag).Equals(frischcards[i]))
                    {
                        alreadyinfrisch = true;
                    }
                }
                if (!alreadyinfrisch)
                {
                    frischcards.Add((Card)label.Tag);
                    int p = label.Location.X;

                    label.Location = new Point(615 + frischcounter * 56, 450);
                    frischlabels[clientid, frischcounter] = label;
                   
                    this.cards.Remove(label);

                    for (int i = 0; i < this.cards.Count; i++)
                    {
                        if (this.cards[i].Location.X > p)
                        {
                            this.cards[i].Location = new Point(this.cards[i].Location.X - 28, 550);
                        }
                        else
                        {
                            this.cards[i].Location = new Point(this.cards[i].Location.X + 28, 550);
                        }
                    }
                    frischcounter++;
                }
            }
            else
            {
                if (frischcounter == 0)
                {
                    if (myICommon.IsCurrneTurn(clientid) && thisroundlabels[clientid].Image == null)
                    {
                        if (myICommon.GetLastWinner() == clientid || !myICommon.DoesHaveShape(clientid))
                        {
                            cards.Remove(label);
                            myICommon.GetLastCcard((Card)label.Tag, clientid);
                            int p = label.Location.X;
                            label.Location = new Point(670, 450);
                            thisroundlabels[clientid] = label;
                            this.cards.Remove(label);

                            for (int i = 0; i < this.cards.Count; i++)
                            {
                                if (this.cards[i].Location.X > p)
                                {
                                    this.cards[i].Location = new Point(this.cards[i].Location.X - 28, 550);
                                }
                                else
                                {
                                    this.cards[i].Location = new Point(this.cards[i].Location.X + 28, 550);
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
                                label.Location = new Point(670, 450);
                                thisroundlabels[clientid] = label;
                                this.cards.Remove(label);

                                for (int i = 0; i < this.cards.Count; i++)
                                {
                                    if (this.cards[i].Location.X > p)
                                    {
                                        this.cards[i].Location = new Point(this.cards[i].Location.X - 28, 550);
                                    }
                                    else
                                    {
                                        this.cards[i].Location = new Point(this.cards[i].Location.X + 28, 550);
                                    }
                                }
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
