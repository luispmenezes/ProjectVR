using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    class Message{
        public int idx;
        public int nVertex;
        public List<List<int>> vertices;

        public Message(int idx, int nVertex) {
            this.idx = idx;
            this.nVertex = nVertex;
            vertices =  new List<List<int>>();
        }

        public void addVertex(int x, int y, int z){
            List<int> vertex = new List<int>();
            vertex.Add(x);
            vertex.Add(y);
            vertex.Add(z);
            vertices.Add( vertex );
        }

        public String[] getVerticies() {
            /*int l = (nVertex / 25);
            String msg[] = new String[l];
            msg[0] = String.Format();*/
            return null;
        }
    }
}
