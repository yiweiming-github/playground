var Navbar = require('../../node_modules/react-bootstrap').Navbar;

const navbarInstance = (
    <Navbar>
        <Navbar.Header>
            <Navbar.Brand>
                <a href="#">react-bootstrap</a>
            </Navbar.Brand>
        </Navbar.Header>
    </Navbar>
);

// 然后我们渲染到body里
ReactDOM.render(navbarInstance,document.body);
