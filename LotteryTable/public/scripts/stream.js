var MarketTable = React.createClass({
  loadMatchListFromServer: function() {
    $.ajax({
      url: this.props.url,
      dataType: 'json',
      cache: false,
      success: function(responseData) {        
        this.setState({data: responseData});
      }.bind(this),
      error: function(xhr, status, err) {        
        console.error(this.props.url, status, err.toString());
      }.bind(this)
    });
  },

  getInitialState: function() {
    var socket = io();
    socket.on('market_quote', this.receiveMessage);

    return {
      data: [],
      socket: socket
    };
  },

  receiveMessage: function(msg) {      
    
    this.setState({data: msg});
  },

  componentDidMount: function() {
    //this.loadMatchListFromServer();
    //this.setState({data: JSON.parse('[{"date":"", "seq":-1, "time":"", "ask":-999.9, "bid": -999.9, "mid": -999.9}]')})    
  },

  render: function() {
    return (
      <div>        
        <MarketQuoteList quotes={this.state.data}/>                
      </div>
    );
  } 
});

var MarketQuote = React.createClass({
  render: function() {
    return (
      <tr>
        <td>{this.props.date}</td>
        <td>{this.props.seq}</td>
        <td>{this.props.time}</td>
        <td>{this.props.code}</td>
        <td>{this.props.ask}</td>
        <td>{this.props.bid}</td>
        <td>{this.props.mid}</td>        
      </tr>
    );
  }
});

var MarketQuoteList = React.createClass({
  render: function() {    
    var quotes = this.props.quotes.map(function(marketQuote) {      
      return (
        <MarketQuote date={marketQuote.date} seq={marketQuote.seq} time={marketQuote.time} code={marketQuote.code} ask={marketQuote.ask} 
        bid={marketQuote.bid} mid={marketQuote.mid}/>
      );
    });

    return (
      <table>
        <tr>
          <th>Date</th>
          <th>Seq</th>
          <th>Time</th>
          <th>Code</th>
          <th>Ask</th>
          <th>Bid</th>
          <th>Mid</th>          
        </tr>
        {quotes}
      </table>
    );    
  }
});

ReactDOM.render(
  <MarketTable url="/api/analysis" pollInterval={5000}/>,
  document.getElementById('content')
);