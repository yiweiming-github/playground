<template>
  <div>
      <h1>Option Strategy Test</h1>
      <div class="container">
        <div class="row">
          <div class="col-sm"/>
          <div class="col-sm">
            <div class="form-group">              
              <input type="text" class="form-control" id="OptionShortCode"  placeholder="Option ShortCode">
              <small id="ShortCodeHelp" class="form-text text-muted">Format should be B/S + Amount + C/P + Strike. For example, "B1C1000" means buy 1 Call option on strike 1000.</small>
            </div>
          </div>
          <div class="col-sm">
            <button type="button" style="float:left" class="btn btn-success" @click="ProcessData()">Add Leg</button>
          </div>
        </div>
      </div>
      <div class="container">
        <div class="table">
     <div class="tableBody">
            <div class="tabRow">
              <div v-for="item in tableHeadData" class="tabCol">{{item.name}}</div>
            </div>
            <div class="tabRow" v-for="item in tableBodyData">
              <div class="tabCol" v-for="tag in item.rows">{{tag.col}}</div>
            </div>
     </div>
     <div class="fixed">
            <div class="fixBtn"><b>操作</b></div>
            <div class="fixBtn" v-for="item in tableBodyData">
              <button  @click="deleteClick(item.id)">删除</button>
            </div>
      </div>
  </div>
      </div>
  </div>
</template>

<script>
export default {
  name: 'PricePanel',
  data () {
    return {
      tableHeadData:[
      {id:1,name:"编号"},
      {id:1,name:"姓名"},
      {id:1,name:"年龄"},
      {id:1,name:"住址"},
      {id:1,name:"电话"},
      {id:1,name:"邮箱"} 
    ],
    tableBodyData:[
       {id:1,
        rows:[{col:1},{col:"张三"},{col:"19"},{col:"长安街118号"},{col:"1234567890"},{col:"1234567890@153.com"}]},
        {id:2,
        rows:[{col:1},{col:"李四"},{col:"19"},{col:"长安街118号"},{col:"1234567890"},{col:"1234567890@153.com"}]} 
    ]
    }
  },
  methods: {
    ProcessData () {
      alert('ProcessData')    
      this.axios.post('http://localhost:8080/greeting', {name: 'John'})
        .then(res => {
            alert(res.data)
            this.message = res.data
        }).catch(res => {
          alert('error')
        })    
    }
  }
}
</script>
