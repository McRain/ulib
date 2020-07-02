namespace UlibPlugins
{
    public class UGatewayPlugin:IGatewayPlugin
    {
        //interface

        public string PluginType
        {
            get { throw new System.NotImplementedException(); }
        }

        public void Activate()
        {
            throw new System.NotImplementedException();
        }

        public void Added()
        {
            throw new System.NotImplementedException();
        }

        public void Remove()
        {
            throw new System.NotImplementedException();
        }

        //abstract

        public virtual string Key
        {
            get { throw new System.NotImplementedException(); }
        }

        public virtual IRemoteObject Gateway
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
