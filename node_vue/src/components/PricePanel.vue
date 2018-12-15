<template>
  <div>
      <h1>this is price panel</h1>
      <div class="form-group">
        <label for="exampleFormControlTextarea1">{{message}}</label>
        <textarea class="form-control" id="exampleFormControlTextarea1" rows="3"></textarea>
    </div>
    <button type="button" class="btn btn-success" @click="ProcessData()">Success</button>
  </div>
</template>

<script>
export default {
  name: 'PricePanel',
  data () {
    return {
      message: ''
    }
  },
  methods: {
    ProcessData () {
      alert('ProcessData')
    //   this.axios.post('http://api.yiliantech.com/AtMoneyOptionQuotes', {maturityDate: '2019-01-14'})
    this.axios.post('http://localhost:8080/greeting', {name: 'John'})
      .then(res => {
        alert(res.data)
        this.message = res.data
      }).catch(res => {
        alert('error')
      })
    //   this.axios({
    //     url: 'http://api.yiliantech.com/AtMoneyOptionQuotes',
    //     method: 'POST',
    //     data: {
    //       maturityDate: '2018-01-14'
    //     },
    //     responseType: 'json'
    //   }).then(function (response) {
    //     alert(response.data)
    //     this.message = response.data
    //   })
    }
  }
}
</script>
