<template>    
    <div class="container">        
        <div class="row">
            <div class="col-sm"/>
            <div class="col-sm">
                <div>
                    <table class="table">
                        <tr>
                            <th>Ticker</th>
                            <th>Price</th>
                        </tr>
                        <tr v-for="data in marketData" v-bind:class="[data.trend > 0 ? 'table-danger' : '', 'table-success']">
                          <td v-text="data.ticker"></td>
                          <td v-text="data.price"></td>
                        </tr>
                    </table>
                </div>
            </div>
            <div class="col-sm"/>
        </div>
    </div>
</template>

<script>
export default {   
    name : 'RealTimeMarket',
    data() {      
        return {        
            websock: null,          
            marketData: ''
      }
    },
    created() {      
        this.initWebSocket();
    },
    destroyed() {      
        this.websock.close() //离开路由之后断开websocket连接
    },    
    methods: {
      initWebSocket(){ //初始化weosocket
        const wsuri = "ws://localhost:8080/websocket/ticker";
        this.websock = new WebSocket(wsuri);
        this.websock.onmessage = this.websocketonmessage;
        this.websock.onopen = this.websocketonopen;
        this.websock.onerror = this.websocketonerror;
        this.websock.onclose = this.websocketclose;
      },
      websocketonopen(){ //连接建立之后执行send方法发送数据
        // let actions = {"test":"12345"};        this.websocketsend(JSON.stringify(actions));
      },
      websocketonerror(){//连接建立失败重连
        this.initWebSocket();
      },
      websocketonmessage(e){ //数据接收
        const redata = JSON.parse(e.data);
        this.marketData = redata;
      },
      websocketsend(Data){//数据发送
        this.websock.send(Data);
      },
      websocketclose(e){  //关闭
        console.log('断开连接',e);
      },
    },
  }
</script>
