var ReactBootstrap = require('react-bootstrap');
var Table = ReactBootstrap.Table;
var Grid = ReactBootstrap.Grid;
var Row = ReactBootstrap.Row;
var Col = ReactBootstrap.Col;
var Checkbox = ReactBootstrap.Checkbox;

var BondListPanel = React.createClass({
    getInitialState: function(){
        return {
            bonds: [
            {'code':'test1', 'coupon':0.03, 'maturity': '2016-10-20', 'type': 'Gov', 'rating': 'AAA'},
            {'code':'test2', 'coupon':0.025, 'maturity': '2016-12-20', 'type': 'Gov', 'rating': 'AAA'},
            {'code':'test3', 'coupon':0.035, 'maturity': '2016-10-20', 'type': 'Corp', 'rating': 'AAA'}
            ]
        }
    },

    render : function(){
        return (
            <Grid>
                <Row>                
                    <Col mdOffset={2} md={8}><BondFilterPanel/></Col>
                </Row>
                <Row>
                    <Col mdOffset={2} md={8}><BondList bonds={this.state.bonds}/></Col>
                </Row>
            </Grid>            
        );
    }
});

var BondFilterPanel = React.createClass({
    render : function(){
        return (
            <Grid>
                <Row>
                    <Col md={1}><Checkbox checked>All</Checkbox></Col>
                    <Col md={1}><Checkbox>Gov</Checkbox></Col>
                    <Col md={1}><Checkbox>Corp</Checkbox></Col>
                </Row>
            </Grid>
        );
    }
});

var BondList = React.createClass({
    render : function(){
        var bonds = this.props.bonds.map(function(bond){
            return (
                <BondRow code={bond.code} coupon={bond.coupon} maturity={bond.maturity} type={bond.type} rating={bond.rating}/>
            );
        });
        return (
            <Table striped bordered condensed hover>
                <thead>
                    <tr>
                        <th>Code</th>
                        <th>Coupon</th>
                        <th>Maturity</th>
                        <th>Type</th>
                        <th>Rating</th>
                    </tr>
                </thead>
                <tbody>
                {bonds}
                </tbody>                
            </Table>
        );
    }
});

var BondRow = React.createClass({
    render : function(){
        return (
            <tr>
                <td>{this.props.code}</td>
                <td>{this.props.coupon}</td>
                <td>{this.props.maturity}</td>
                <td>{this.props.type}</td>
                <td>{this.props.rating}</td>                        
            </tr>
        );
    }
});

ReactDOM.render(
  <BondListPanel/>,
  document.getElementById('content')
);