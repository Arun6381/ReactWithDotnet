import React, { useState } from "react";
import axios from "axios";

const DeleteProductItem = ({ productId, onDeleteComplete }) => {
    const [error, setError] = useState("");

    const handleDelete = async () => {
        if (window.confirm("Are you sure you want to delete this product item?")) {
            try {
                await axios.delete(`https://localhost:7217/api/productitems/${productId}`);
                onDeleteComplete();
            } catch (err) {
                setError("Failed to delete product item.");
            }
        }
    };

    return (
        <div>
            {error && <div className="alert alert-danger">{error}</div>}
            <button className="btn btn-danger" onClick={handleDelete}>
                Delete
            </button>
        </div>
    );
};

export default DeleteProductItem;
