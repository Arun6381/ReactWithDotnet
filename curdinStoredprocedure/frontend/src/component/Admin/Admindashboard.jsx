import React, { useEffect, useState } from "react";
import { FaArrowLeft, FaArrowRight } from "react-icons/fa";
import { MdCancel } from "react-icons/md";
import axios from "axios";
import "./Admin.css";

const CartDetails = () => {
    const [cartDetails, setCartDetails] = useState([]);
    const [usernames, setUsernames] = useState([]);
    const [filteredUsernames, setFilteredUsernames] = useState([]);
    const [selectedUser, setSelectedUser] = useState("");
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 10; 

    useEffect(() => {
        const fetchCartDetails = async () => {
            try {
                const response = await axios.get(
                    "https://localhost:7217/api/AddToCart/GetCartDetail",
                    { withCredentials: true }
                );

                const data = response.data;
                if (data.length === 0) {
                    setError("No cart items found.");
                } else {
                    setCartDetails(data);

                    const uniqueUsernames = [...new Set(data.map((item) => item.firstName))];
                    setUsernames(uniqueUsernames);
                }
            } catch (err) {
                setError(err.response?.data || "Error fetching cart details");
            } finally {
                setLoading(false);
            }
        };

        fetchCartDetails();
    }, []);

    const handleInputChange = (event) => {
        const input = event.target.value;
        setSelectedUser(input);

        if (input) {
            const suggestions = usernames.filter((username) =>
                username.toLowerCase().includes(input.toLowerCase())
            );
            setFilteredUsernames(suggestions);
        } else {
            setFilteredUsernames([]);
        }
    };

    const handleSuggestionClick = (username) => {
        setSelectedUser(username);
        setFilteredUsernames([]);
    };

    const filteredCartDetails = selectedUser
        ? cartDetails.filter((item) =>
            item.firstName.toLowerCase().includes(selectedUser.toLowerCase())
        )
        : cartDetails;

    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentItems = filteredCartDetails.slice(indexOfFirstItem, indexOfLastItem);

    const totalPages = Math.ceil(filteredCartDetails.length / itemsPerPage);

    const handleNextPage = () => {
        if (currentPage < totalPages) setCurrentPage(currentPage + 1);
    };

    const handlePreviousPage = () => {
        if (currentPage > 1) setCurrentPage(currentPage - 1);
    };

   

    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>{error}</div>;
    }

    return (
        <>
        <div className="cart-container">
                <div className="filter-container">
                    <div style={{display:"flex"}}>  <input
                        type="text"
                        id="user-filter"
                        value={selectedUser}
                        onChange={handleInputChange}
                        placeholder="Type a username..."
                    />
                        {selectedUser!=="" ?  <span className="clear-btn"
                            style={{ borderRadius: '10px' }}
                            onClick={() => {
                                setSelectedUser("")
                            }}><MdCancel />
                        </span>
                            : ""}
                    </div>
              
                    {filteredUsernames.length > 0 && selectedUser !== ""?  (
                    <ul className="suggestion-list">
                        {filteredUsernames.map((username, index) => (
                            <li key={index} onClick={() => handleSuggestionClick(username)}>
                                {username}
                            </li>
                        ))}
                    </ul>
                    ) : ""}
            </div>

                <div className="cart-table">
                    <div className="cart-header">
                        <div className="header-item">User Name</div>
                        <div className="header-item">Product</div>
                        <div className="header-item">Price</div>
                        <div className="header-item">Status</div>
                    </div>
                    <div className="cart-body">
                        {currentItems.length === 0 ? (
                            <p className="no-items">No items found for the selected user</p>
                        ) : (
                            currentItems.map((item, index) => (
                                <div className="cart-row" key={index}>
                                    <div className="cart-cell">{item.firstName}</div>
                                    <div className="cart-cell">{item.productName}</div>
                                    <div className="cart-cell">${item.price}</div>
                                    <div className="cart-cell">
                                        {item.status === 1 ? "Order Placed" : "Pending"}
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </div>

          
            </div>
            <div className="pagination-controls">
                <button
                    onClick={handlePreviousPage}
                    disabled={currentPage === 1}
                    className="pagination-button"
                >
                    <FaArrowLeft />

                </button>
                <span className="pagination-info">
                     {currentPage} / {totalPages}
                </span>
                <button
                    onClick={handleNextPage}
                    disabled={currentPage === totalPages}
                    className="pagination-button"
                >
                    <FaArrowRight />

                </button>
            </div>
        </>
    );
};

export default CartDetails;
