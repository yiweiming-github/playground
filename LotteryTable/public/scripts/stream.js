var MatchTable = React.createClass({
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
    socket.on('chat message', this.receiveMessage);

    return {
      data: [],
      socket: socket
    };
  },

  receiveMessage: function(msg) {       
    // var msgs = this.state.data;
    // if(msgs.length > 20)
    // {
    //   msgs.shift();
    // }
    // msgs.push(msg);
    //this.setState({data: msgs});
    this.setState({data: msg});
  },

  componentDidMount: function() {
    this.loadMatchListFromServer();
    //setInterval(this.loadMatchListFromServer, this.props.pollInterval);
  },

  render: function() {
    return (
      <div>        
        <MatchList matches={this.state.data}/>                
      </div>
    );
  } 
});

var Match = React.createClass({
  render: function() {
    return (
      <tr>
        <td>{this.props.date}</td>
        <td>{this.props.homeTeam}</td>
        <td>{this.props.awayTeam}</td>
        <td>{this.props.win}</td>
        <td>{this.props.draw}</td>
        <td>{this.props.lose}</td>
        <td>{this.props.predict}</td>
        <td>{this.props.result}</td>
      </tr>
    );
  }
});

var MatchList = React.createClass({
  render: function() {    
    var matches = this.props.matches.map(function(matchItem) {      
      return (
        <Match date={matchItem.date} homeTeam={matchItem.homeTeam} awayTeam={matchItem.awayTeam} win={matchItem.win} 
        draw={matchItem.draw} lose={matchItem.lose} predict={matchItem.predict} result={matchItem.result}/>
      );
    });

    return (
      <table>
        <tr>
          <th>Date</th>
          <th>Home Team</th>
          <th>Away Team</th>
          <th>Win</th>
          <th>Draw</th>
          <th>Lose</th>
          <th>Predict</th>
          <th>Result</th>
        </tr>
        {matches}
      </table>
    );    
  }
});

ReactDOM.render(
  <MatchTable url="/api/analysis" pollInterval={5000}/>,
  document.getElementById('content')
);