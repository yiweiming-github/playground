var ReactBootstrap = require('react-bootstrap');
var Table = ReactBootstrap.Table;
var Grid = ReactBootstrap.Grid;
var Row = ReactBootstrap.Row;
var Col = ReactBootstrap.Col;
var Checkbox = ReactBootstrap.Checkbox;

var BondListPanel = React.createClass({
    loadBondListFromServer: function () {
        $.ajax({
            url: this.props.url,
            dataType: 'json',
            cache: false,
            success: function (responseData) {
                this.setState({ bonds: responseData });
            }.bind(this),
            error: function (xhr, status, err) {
                console.error(this.props.url, status, err.toString());
            }.bind(this)
        });
    },

    getInitialState: function () {
        return {
            bonds: []
        }
    },

    componentDidMount: function() {
        this.loadBondListFromServer();
    },

    render: function () {
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
    render: function () {
        return (
            <Grid>
                <Row>
                    <BondFilter condition="All"/>
                    <BondFilter condition="Gov"/>
                    <BondFilter condition="Corp"/>
                </Row>
            </Grid>
        );
    },    
});

var BondFilter = React.createClass({
    render: function () {
        var onClickEvent = this.clickHandler.bind(this, this.props.condition);        
        return (
            <Col md={1}><Checkbox onClick={onClickEvent}>{this.props.condition}</Checkbox></Col>
        );
    },

    clickHandler: function (condition){
        this.dispatchEvent('filterBonds', condition);
    }
});

var BondList = React.createClass({
    render: function () {
        var bonds = this.props.bonds.map(function (bond) {
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
    render: function () {
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
    <BondListPanel url="/api/bondlist"/>,
    document.getElementById('content')
);