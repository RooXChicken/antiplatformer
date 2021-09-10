namespace antiplatformer
{
    public class entity
    {
        public dynamic myClass;
        public entity(string name, string paramaters)
        {
            switch (name.ToLower())
            {
                case "player":
                    myClass = new entityLogic.playerEntity(paramaters);
                    myClass.onSpawn();
                    break;
                case "splashscreen":
                    myClass = new entityLogic.splashText();
                    myClass.onSpawn();
                    break;
                case "text":
                    myClass = new entityLogic.textEntity(paramaters);
                    myClass.onSpawn();
                    break;
                case "decor":
                    myClass = new entityLogic.decoration(paramaters);
                    myClass.onSpawn();
                    break;
                case "fadetoblack":
                    myClass = new entityLogic.fadeToBlack();
                    myClass.onSpawn();
                    break;
                case "darken":
                    myClass = new entityLogic.overlay();
                    myClass.onSpawn(paramaters);
                    break;
                case "king":
                    myClass = new entityLogic.king();
                    myClass.onSpawn(paramaters);
                    break;
                case "checkpoint":
                    myClass = new entityLogic.checkpoint(paramaters);
                    myClass.onSpawn();
                    break;
                case "portal":
                    myClass = new entityLogic.endLevelPortal(paramaters);
                    myClass.onSpawn();
                    break;
                case "dustsprite":
                    myClass = new entityLogic.dustSprite(paramaters);
                    myClass.onSpawn();
                    break;
                default:
                    myClass = new entityLogic.errorEntity();
                    myClass.onSpawn();
                    break;
            }
        }
    }
}
