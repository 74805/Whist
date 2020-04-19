using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using Common;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //213.57.202.58
            InitializeComponent();
            HttpChannel chnl = new HttpChannel(1234);
            ChannelServices.RegisterChannel(chnl, false);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ServerPart),
                "_Server_",
                WellKnownObjectMode.Singleton);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    class ServerPart : MarshalByRefObject, ICommon
    {
        private List<Card> packet = new List<Card>();
        private List<Card>[] pcards = new List<Card>[4];
        private int counter = 0;
        private int currenturn = -1;
        private int[] bet = new int[4];
        private string[] bet1 = new string[4];
        string[] pnames = new string[4];
        private List<Card>[] usedcards = new List<Card>[4];
        private int counter3 = 0;
        private Card[] thisroundcards = new Card[4];
        private int thisid;
        string trump;
        private int firstplayer = -4;
        private int winnerid;
        private int removelabels = 0;
        private int counter4 = 0;
        private bool[] removecards = new bool[4];
        private bool[] nextgame = new bool[4];
        private bool declare;
        private string[] declares = new string[4];
        private bool[] declareover = new bool[4];
        private string[] trumps = new string[4];
        private bool[] frisch = new bool[4];
        private bool newcard;
        private bool[] gotnewcard;
        private bool[] endofround;
        private bool[] getdeclare = new bool[4];
        private string thisdeclare;
        private int declarestarter = 0;
        private Card[,] frischcards = new Card[4, 3];
        private List<bool>[] gotnewfrischcard = new List<bool>[4];
        private List<bool>[] tookcard;
        private int counter1 = 0;
        private bool redeclare;
        public ServerPart()
        {
            Packet();
        }
        public void Packet()
        {//creating the packet sorted by numbers. 
            for (int i = 0; i < 4; i++)
            {
                pcards[i] = new List<Card>();
            }
            for (int i = 0; i < 52; i++)
            {
                if (i % 4 == 0)
                {
                    if (i / 4 + 1 == 1)
                    {
                        packet.Add(new Card(14, "hearts"));
                    }
                    else
                    {
                        packet.Add(new Card(i / 4 + 1, "hearts"));
                    }
                }
                else
                {
                    if (i % 4 == 1)
                    {
                        if (i / 4 + 1 == 1)
                        {
                            packet.Add(new Card(14, "clubs"));
                        }
                        else
                        {

                            packet.Add(new Card(i / 4 + 1, "clubs"));
                        }
                    }
                    else
                    {
                        if (i % 4 == 2)
                        {
                            if (i / 4 + 1 == 1)
                            {
                                packet.Add(new Card(14, "diamonds"));
                            }
                            else
                            {
                                packet.Add(new Card(i / 4 + 1, "diamonds"));
                            }
                        }
                        else
                        {
                            if (i % 4 == 3)
                            {
                                if (i / 4 + 1 == 1)
                                {
                                    packet.Add(new Card(14, "spades"));
                                }
                                else
                                {
                                    packet.Add(new Card(i / 4 + 1, "spades"));
                                }
                            }
                        }


                    }
                }

            }
            Random rnd = new Random();

            int card;
            //shuffling the packet
            for (int i = 0; i < 52; i++)
            {
                card = rnd.Next(1, 52);

                if (packet[card].GetNum() == 0)
                {
                    i--;
                }
                else
                {
                    pcards[i % 4].Add(new Card(packet[card]));
                    packet[card].SetNum(0);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                usedcards[i] = new List<Card>();
            }
        }
        public int[] CardNum()
        {
            int[] arr = new int[4];
            for (int i = 0; i < 4; i++)
            {
                arr[i] = pcards[i].Count;
            }
            return arr;
        }

        public void doturn1(int bet, string shape, int clientid)
        {
            /*         counter2++;
                     if (shape == null || bet == 0)
                     {
                         counter1++;
                     }
                     else
                     {
                         counter1 = 0;
                         this.bet[clientid] = bet;
                         this.bet1[clientid] = shape;
                     }

                     if (counter == 3&&counter2>=4)
                     {
                         Trump();
                     }*/
            // counter++;
            // nextplayerturn();
        }

        public void doturn2(Card card, int clientid)
        {
            for (int i = 0; i < pcards[clientid].Count; i++)
            {
                if (pcards[clientid][i] == card)
                {
                    pcards[clientid][i] = pcards[clientid][pcards[clientid].Count - 1];
                    pcards[clientid].RemoveAt(pcards[clientid].Count - 1);
                }
            }
            nextplayerturn();
        }


        public bool GetFrisch(int clientid)
        {
            return frisch[clientid];
        }
        public Card[] GetCards(int ClientId)
        {
            /* Card[] pcard = new Card[13];

             for (int i = 0; i < 13; i++)
             {
                 pcard[i] = new Card(pcards[ClientId][i]);
             }
             return pcard;*/
            int spa = 0, dia = 0, hea = 0;

            List<Card> spad = new List<Card>();
            List<Card> hear = new List<Card>();
            List<Card> diam = new List<Card>();
            List<Card> clu = new List<Card>();
            Card[] cards1 = new Card[13];
            for (int i = 0; i < pcards[ClientId].Count; i++)
            {
                if (pcards[ClientId][i].GetShape() == "spades")
                {
                    spad.Add(pcards[ClientId][i]);
                    spa++;
                }
                if (pcards[ClientId][i].GetShape() == "hearts")
                {
                    hear.Add(pcards[ClientId][i]);
                    hea++;
                }
                if (pcards[ClientId][i].GetShape() == "diamonds")
                {
                    diam.Add(pcards[ClientId][i]);
                    dia++;
                }
                if (pcards[ClientId][i].GetShape() == "clubs")
                {
                    clu.Add(pcards[ClientId][i]);
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
                cards1[i + 13 - dia] = diam[i];
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
                cards1[i + spa + hea] = clu[i];
            }
            pcards[ClientId] = cards1.ToList<Card>();
            return cards1;
        }

        public void SetFirsttPlayer(int clientid)
        {
            currenturn = clientid;
            firstplayer = clientid;
            winnerid = clientid;
        }

        public bool GetNames()
        {
            int counter = 0;
            for (int i = 0; i < 4; i++)
            {
                if (pnames[i] != null)
                {
                    counter++;
                }
            }
            if (counter == 4)
            {
                return true;
            }
            return false;

        }

        public int Id()
        {
            counter++;
            return counter - 1;
        }

        public bool IsCurrneTurn(int ClientId)
        {
            if (currenturn == ClientId)
            {
                return true;
            }
            return false;
        }

        public Card Lastcard()
        {
            Card card = new Card(13, "spades");//כדי שלא יהיה ארור
            return card;
        }

        public void Name(int clientid, string name)
        {
            try
            {
                pnames[clientid] = name;
            }
            catch
            {

            }
        }

        public void nextplayerturn()
        {
            if (currenturn == 3)
            {
                currenturn = 0;
            }
            else
            {
                currenturn++;
            }
        }

        public string Trump()
        {
            List<int> list = new List<int>();
            int counter = 0;
            int clientid = -1;
            for (int i = 13; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (this.bet[j] == i)
                    {
                        clientid = j;
                        list.Add(j);
                        counter++;
                    }
                }
                if (counter > 0)
                {
                    break;
                }
                else
                {
                    return bet1[clientid];
                }
            }

            for (int i = 0; i < counter; i++)
            {

            }
            return "";//כדי שלא יהיה ארור
        }

        public void NewRoundCards()
        {
            for (int i = 0; i < 4; i++)
            {
                thisroundcards[i] = null;
            }
        }
        public void GetLastCcard(Card card, int clientid)
        {
            thisid = clientid;
            usedcards[clientid].Add(card);
            gotnewcard = new bool[4];
            foreach (Card card1 in pcards[clientid])
            {
                if (card.GetNum() == card1.GetNum() && card.GetShape() == card1.GetShape())
                {
                    pcards[clientid].Remove(card1);
                    break;
                }
            }
            counter3++;
            if (counter3 == 4)
            {
                //המנצח מתחיל בסיבוב הבא - לכתוב
                counter3 = 0;
                NewRoundCards();

            }
            else
            {
                counter3++;
                ThisRoundCards();
                nextplayerturn();
            }
        }
        public Card[] ThisRoundCards()
        {
            if (usedcards[thisid] != null && usedcards[thisid].Count != 0 && winnerid != currenturn)
            {

                thisroundcards[thisid] = usedcards[thisid][usedcards[thisid].Count - 1];
            }
            return thisroundcards;
        }

        public void SetTrump(string trump)
        {
            this.trump = trump;
        }

        public int Getfirstid()
        {
            return firstplayer;
        }

        public bool DoesHaveShape(int clientid)
        {
            foreach (Card card in pcards[clientid])
            {
                if (currenturn == winnerid || (card.GetShape() == thisroundcards[winnerid].GetShape()))
                {
                    return true;
                }
            }
            return false;
        }

        public string RoundShape()
        {
            return thisroundcards[winnerid].GetShape();
        }

        public bool IsCorrectShape(Card card)
        {
            if (thisroundcards[winnerid] != null && card.GetShape() == thisroundcards[winnerid].GetShape())
            {
                return true;
            }
            return false;
        }

        public List<Card> Pcards(int clientid)
        {
            return pcards[clientid];
        }

        public void WinnerId(int clientid)
        {
            endofround = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                endofround[i] = true;
            }

            winnerid = clientid;
            currenturn = clientid;


            if (removelabels - counter4 == 0 || usedcards[0].Count == 0)
            {
                removelabels += 4;
            }
            RemoveCards();
        }

        public bool GetRemoveLabels()
        {
            if (removelabels > counter4)
            {

                counter4++;
                return true;
            }
            return false;
        }
        public void RemoveCards()
        {
            for (int i = 0; i < 4; i++)
            {
                thisroundcards[i] = null;
            }
        }

        public void CheckWinner()
        {

            int trumps = 0;
            int firstshape = 0;
            Card[] trumpcards = new Card[4];
            Card[] firstshapecards = new Card[4];
            for (int i = 0; i < 4; i++)
            {
                if (thisroundcards[i] != null && thisroundcards[i].GetShape() == trump)
                {
                    trumps++;
                    trumpcards[i] = thisroundcards[i];
                }
                else
                {
                    if (thisroundcards[i] != null && IsCorrectShape(thisroundcards[i]))
                    {
                        firstshape++;
                        firstshapecards[i] = thisroundcards[i];
                    }
                }
            }
            int winnercardnum = 0;
            if (trumps > 0)
            {
                for (int i = 0; i < 4; i++)
                {

                    if (trumpcards[i] != null && trumpcards[i].GetNum() > winnercardnum)
                    {
                        winnercardnum = trumpcards[i].GetNum();
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    if (trumpcards[i] != null && trumpcards[i].GetNum() == winnercardnum)
                    {
                        winnerid = i;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {

                    if (firstshapecards[i] != null && firstshapecards[i].GetNum() > winnercardnum)
                    {
                        winnercardnum = firstshapecards[i].GetNum();
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    if (firstshapecards[i] != null && firstshapecards[i].GetNum() == winnercardnum)
                    {

                        winnerid = i;


                    }
                }
            }

        }


        public int GetWinnerId()
        {
            CheckWinner();
            return winnerid;
        }
        public int GetLastWinner()
        {
            return winnerid;
        }
        public Card[] UsedCards()
        {

            return usedcards[0].ToArray();
        }

        public void SetRemoveCardsTrue()
        {
            for (int i = 0; i < 4; i++)
            {
                removecards[i] = true;
            }

        }


        public bool GetRemoveCards(int clienid)
        {
            return removecards[clienid];
        }

        public void SetRemoveCardsFalse(int clientid)
        {
            removecards[clientid] = false;
        }

        public void SetNextGame()
        {
            packet = new List<Card>();
            Packet();
            trump = null;
            counter = 0;
            currenturn = -1;
            bet = new int[4];
            bet1 = new string[4];
            usedcards = new List<Card>[4];
            counter3 = 0;
            thisroundcards = new Card[4];
            firstplayer = -4;
            removelabels = 0;
            counter4 = 0;
            removecards = new bool[4];
            declare = false;
            declares = new string[4];
            declareover = new bool[4];
            trumps = new string[4];
            frisch = new bool[4];
            newcard = false;
            gotnewcard = new bool[4];
            endofround = new bool[4];
            getdeclare = new bool[4];
            thisdeclare = null;
            declarestarter = 0;
            frischcards = new Card[4, 3];
            gotnewfrischcard = new List<bool>[4];
            if (declarestarter == 3)
            {
                declarestarter = 0;
            }
            else
            {
                declarestarter++;
            }
            for (int i = 0; i < 4; i++)
            {
                usedcards[i] = new List<Card>();
                nextgame[i] = true;
            }


        }

        public bool GetNextGame(int clientid)
        {
            if (clientid == -1)
            {
                return false;
            }
            return nextgame[clientid];
        }

        public void SetNextGameFalse(int clientid)
        {
            nextgame[clientid] = false;
        }

        public void SetDeclare()
        {
            declare = true;
            currenturn = declarestarter;
        }

        public bool GetDeclare(int clientid)
        {
            return declare && (IsCurrneTurn(clientid));
        }

        public void updatedeclare(int clientid, string declare, string trump)
        {

            int counter = 0;
            int counter1 = 0;
            declares[clientid] = declare;
            trumps[clientid] = trump;
            thisdeclare = declare + " " + (trump == "pass" ? "" : trump) + clientid;

            if (declares[clientid] != "Pass")
            {
                int counter8 = 0;
                for (int i = clientid; counter8 != 4; i--)
                {
                    if (declares[i] == "Pass")
                    {
                        declares[i] = null;
                        counter8 = 0;
                    }
                    else
                    {
                        counter8++;
                    }
                    if (i == 0)
                    {
                        i = 4;
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (declares[i] == null)
                {
                    counter1++;
                }
                else
                {
                    if (declares[i] == "Pass")
                    {
                        counter++;
                    }
                }
            }
            if (counter == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    frisch[i] = true;
                }
                redeclare = false;
            }
            if (counter == 3 && counter1 == 0)
            {
                this.declare = false;
                for (int i = 0; i < 4; i++)
                {
                    declareover[i] = true;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (declares[i] != "Pass")
                    {
                        this.trump = trumps[i];
                        currenturn = i;
                        winnerid = i;
                    }
                }
            }
            if (this.trump == null)
            {
                nextplayerturn();
            }
            for (int i = 0; i < 4; i++)
            {
                getdeclare[i] = true;
            }
        }

        public int GetTurn()
        {
            return currenturn;
        }
        public bool NewCard(int clientid)
        {
            if (gotnewcard != null && !gotnewcard[clientid])
            {
                gotnewcard[clientid] = true;
                return true;
            }
            return false;
        }

        public string[] PlayersNames()
        {
            return pnames;
        }

        public bool ThisRoundPlays()
        {
            int counter = 0;
            for (int i = 0; i < 4; i++)
            {
                if (thisroundcards[i] != null)
                {
                    counter++;
                }
            }
            if (counter == 4)
            {
                return true;
            }
            return false;
        }

        public bool isroundover(int clientid)
        {
            if (endofround != null && endofround[clientid])
            {
                endofround[clientid] = false;
                return true;
            }
            return false;
        }

        public bool NewDeclare(int clientid)
        {
            if (getdeclare[clientid])
            {
                getdeclare[clientid] = false;
                return true;
            }
            return false;
        }

        public string ThisDeclare()
        {
            return thisdeclare;
        }

        public bool IsDeclareOver(int clientid)
        {
            if (declareover[clientid])
            {
                declareover[clientid] = false;
                return true;
            }
            return false;
        }



        public void NewFrischCard(Card card, int clientid)
        {
            for (int i = 0; i < pcards[clientid].Count; i++)
            {
                if (pcards[clientid][i].GetNum() == card.GetNum() && pcards[clientid][i].GetShape() == card.GetShape())
                {
                    pcards[clientid].RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (clientid != i)
                {
                    if (gotnewfrischcard[i] == null)
                    {
                        gotnewfrischcard[i] = new List<bool>();
                    }
                    gotnewfrischcard[i].Add(true);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (frischcards[clientid, i] == null)
                {
                    if (i == 2)
                    {
                        frisch[clientid] = false;
                    }
                    frischcards[clientid, i] = card;
                    break;
                }

            }
        }

        public Card[,] GetFrischCards()
        {
            return frischcards;
        }

        public bool IsNewFrischCard(int clientid)
        {
            try
            {
                if (gotnewfrischcard[clientid] != null && gotnewfrischcard[clientid][gotnewfrischcard[clientid].Count - 1])
                {
                    gotnewfrischcard[clientid].Remove(gotnewfrischcard[clientid][gotnewfrischcard[clientid].Count - 1]);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public int GetCardIndex(Card card, int clientid)
        {
            if (tookcard == null)
            {
                tookcard = new List<bool>[4];

            }
            for (int i = 0; i < 4; i++)
            {
                tookcard[i] = new List<bool>();
                tookcard[i].Add(true);
            }

            pcards[clientid].Add(card);
            string shape = card.GetShape();
            for (int i = 0; i < 3; i++)
            {
                if (frischcards[(clientid + 1) == 4 ? 0 : (clientid + 1), i] != null && frischcards[(clientid + 1) == 4 ? 0 : (clientid + 1), i].GetNum() == card.GetNum() && frischcards[(clientid + 1) == 4 ? 0 : (clientid + 1), i].GetShape() == shape)
                {
                    frischcards[(clientid + 1) == 4 ? 0 : (clientid + 1), i] = new Card(0, "a");
                }
            }


            for (int i = 0; i < pcards[clientid].Count; i++)
            {
                if (pcards[clientid][i].GetShape() == shape && pcards[clientid][i].GetNum() != card.GetNum())
                {
                    if (pcards[clientid][i].GetNum() < card.GetNum())
                    {

                        for (int j = i; j < pcards[clientid].Count; j++)
                        {
                            Card card1 = new Card(pcards[clientid][j]);
                            pcards[clientid][j] = pcards[clientid][pcards[clientid].Count - 1];
                            pcards[clientid][pcards[clientid].Count - 1] = card1;
                        }
                        return i;
                    }
                    if (pcards[clientid][i + 1 == (pcards[clientid].Count) ? 0 : (i + 1)].GetShape() != shape)
                    {
                        for (int j = i; j < pcards[clientid].Count - 1; j++)
                        {
                            Card card1 = new Card(pcards[clientid][j + 1]);
                            pcards[clientid][j + 1] = pcards[clientid][pcards[clientid].Count - 1];
                            pcards[clientid][pcards[clientid].Count - 1] = card1;
                        }
                        return i+1;
                    }
                }
            }

            if (shape == "spades")
            {


                for (int i = 0; i < pcards[clientid].Count; i++)
                {
                    Card card1 = new Card(pcards[clientid][i]);
                    pcards[clientid][i] = pcards[clientid][pcards[clientid].Count - 1];
                    pcards[clientid][pcards[clientid].Count - 1] = card1;
                }
                return 0;
            }
            if (shape == "hearts")
            {

                for (int i = 0; i < pcards[clientid].Count; i++)
                {
                    if (pcards[clientid][i].GetShape() != "spades")
                    {
                        for (int j = i; i < pcards[clientid].Count; i++)
                        {
                            Card card1 = new Card(pcards[clientid][j]);
                            pcards[clientid][j] = pcards[clientid][pcards[clientid].Count - 1];
                            pcards[clientid][pcards[clientid].Count - 1] = card1;
                        }
                        return i;
                    }
                }

            }
            if (shape == "clubs")
            {

                for (int i = 0; i < pcards[clientid].Count; i++)
                {
                    if (pcards[clientid][i].GetShape() != "spades" && pcards[clientid][i].GetShape() != "diamonds")
                    {
                        for (int j = i; i < pcards[clientid].Count; i++)
                        {
                            Card card1 = new Card(pcards[clientid][j]);
                            pcards[clientid][j] = pcards[clientid][pcards[clientid].Count - 1];
                            pcards[clientid][pcards[clientid].Count - 1] = card1;
                        }
                        return i;
                    }
                }

            }
            if (shape == "diamonds")
            {

                return pcards[clientid].Count - 1;

            }
            return 0;

        }

        public int[] TookCard(int clientid)
        {

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (frischcards[i, j] != null && frischcards[i, j].GetNum() == 0)
                    {
                        counter1++;
                        if (counter1 % 4 == 0)
                        {
                            frischcards[i, j] = new Card(1, "a");
                        }
                        return new int[] { i, j };
                    }
                }
            }
            return new int[] { 0, 0 };
        }

        public bool RemoveFrischCard(int clientid)
        {
            if (tookcard != null && tookcard[clientid].Count != 0 && tookcard[clientid][tookcard[clientid].Count - 1])
            {
                tookcard[clientid].RemoveAt(tookcard[clientid].Count - 1);
                return true;
            }
            return false;
        }

        public bool AreAllCardsTaken()
        {

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (frischcards[i,j] != null && frischcards[i, j].GetNum() != 1)
                    {
                        return false;
                    }
                }
            }
            if (!redeclare&&!frisch[0])
            {
                redeclare = true;
                SetDeclare();
            }
            return true;
        }
    }
}