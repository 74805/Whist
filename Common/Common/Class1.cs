using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;


namespace Common
{
    public interface ICommon
    {
        Card[] GetCards(int ClientId);
        void doturn1(int bet, string shape,int clientid);
        void doturn2(Card card, int clientid);
        int Id();
        bool IsCurrneTurn(int clientid);
        Card Lastcard();
        int[] CardNum();
        string Trump();
        void Name(int clientid, string name);

        bool GetNames();

        void SetFirsttPlayer(int clientid);

        void GetLastCcard(Card card, int clientid);       
        Card[] ThisRoundCards();

        void SetTrump(string trump);

        int Getfirstid();

        bool DoesHaveShape(int clientid);
        string RoundShape();
        bool IsCorrectShape(Card card);
        List<Card> Pcards(int clientid);
        void WinnerId(int clientid);
        bool GetRemoveLabels();

        void CheckWinner();
        int GetWinnerId();
        Card[] UsedCards();
        int GetLastWinner();
        
        void SetRemoveCardsTrue();
        bool GetRemoveCards(int clientid);
        void SetRemoveCardsFalse(int clientid);
        void SetNextGame();
        bool GetNextGame(int clientid);
        void SetNextGameFalse(int clientid);
        void SetDeclare();
        bool GetDeclare(int clientid);
        void updatedeclare(int clientid,string declare,string trump);
        bool GetFrisch(int clientid);
        int GetTurn();
        bool NewCard(int clientid);
        string[] PlayersNames();
        bool ThisRoundPlays();
        bool isroundover(int clientid);
        bool NewDeclare(int clientid);
        string ThisDeclare();
        bool IsDeclareOver(int clientid);
        void NewFrischCard(Card card,int clientid);
        Card[,] GetFrischCards();
        bool IsNewFrischCard(int clientid);
        int GetCardIndex(Card card,int clientid);
        int[] TookCard(int clientid);
        bool RemoveFrischCard(int clientid);
        bool AreAllCardsTaken();
    }

    [Serializable]
    public class Card
    {
        private int num;
        private string shape;

        public Card(int num, string shape)
        {
            this.num = num;
            this.shape = shape;
        }

        public Card(Card other)
        {
            this.num = other.num;
            this.shape = other.shape;
        }
        public int GetNum()
        {
            return this.num;
        }

        public string GetShape()
        {
            return this.shape;
        }

        public void SetNum(int num)
        {
            this.num = num;
        }

        public void SetShape(string shape)
        {
            this.shape = shape;
        }

        public override string ToString()
        {
            return this.num + " - " + this.shape;
        }

        
    }
}