import React, { useState } from "react";
import axios from "axios";

const DeleteCategory = ({ categoryId, onDeleteComplete }) => {
    const [error, setError] = useState("");
    console.log(categoryId)
    const handleDelete = async () => {
        if (window.confirm("Are you sure you want to delete this category?")) {
            try {
                await axios.delete(`https://localhost:7217/api/productcategories/${categoryId}`);
               onDeleteComplete(); // Notify parent to refresh categories
            } catch (err) {
                setError("Failed to delete category.");
            }
        }
    };

    return (
        <div>
            {error && <div className="alert alert-danger">{error}</div>}
            <button className="btn btn-danger" onClick={handleDelete}>
                Confirm Delete
            </button>
        </div>
    );
};

export default DeleteCategory;
