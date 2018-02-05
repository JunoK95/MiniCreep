using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.InDev
{
    public class Bob_Ai : Figure
    {
        public GameObject CurrentHexTile;
        public Vector2 _hex = new Vector2(2, 2);
        public GameObject Self;
        public Material NoHigh;
        public Material High;
        private bool moved = false;
        private Renderer r;
        void Start()
        {

            theGame = theGameObject.GetComponent<GameState>();
            theGame.ActionPhase += this.Action;
            

        }
        private void Update()
        {
            if (!moved) MoveToHex();
            
        }
        public override void Action()
        {
            int r = UnityEngine.Random.Range(0, 6);
            switch(r)
            {
                case 5:
                    Attack(new Vector2(_hex.x + 1, _hex.y));
                    
                    break;
                case 4:
                    Attack(new Vector2(_hex.x , _hex.y+1));
                    break;
                case 3:
                    Attack(new Vector2(_hex.x-1, _hex.y + 1));
                    break;
                case 2:
                    Attack(new Vector2(_hex.x-1, _hex.y ));
                    break;
                case 1:
                    Attack(new Vector2(_hex.x - 1, _hex.y - 1));
                    break;
                case 0:
                    Attack(new Vector2(_hex.x , _hex.y - 1));
                    break;

            }
            Debug.Log(r);
           // Debug.Log("Action was successfully activated");
           // theGame.ActionPhase -= this.Action;
        }
        private void Attack(Vector2 hex)
        {
            if(r!=null)
            r.material = NoHigh;
            GameObject _currentHexTile = GameObject.Find("Board").GetComponent<GameBoard>()._HexGrid[(int)hex.x,(int)hex.y]._Contents[0];
            r =_currentHexTile.transform.Find("pCylinder1").GetComponent<Renderer>();
            NoHigh = r.material;
            r.material = High;
            Debug.Log("bob attacks"+hex.x+":"+hex.y);
        }
        public void MoveToHex()
        {
            
            theGame.addFigure(this.gameObject, 2,2);
           
                  // Debug.Log("moveing to:"+MoveTo.ToString());
                if (transform.position != MoveTo)
                {
                    transform.position = MoveTo;
                


            }

        }

        public override void Movement()
        {
            throw new NotImplementedException();
        }

        public override void Death()
        {
            
        }
    }
}
