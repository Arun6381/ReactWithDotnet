import React, { useEffect, useState } from "react";
import axios from "axios";
import {
    PieChart,
    Pie,
    Cell,
    BarChart,
    Bar,
    XAxis,
    YAxis,
    Tooltip,
    Legend,
    ResponsiveContainer
} from "recharts";
import { utils, writeFile } from "xlsx";
import { toPng } from "html-to-image";
import "./Chart.css";

const CartDetails = () => {
    const [cartDetails, setCartDetails] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchCartDetails = async () => {
            try {
                const response = await axios.get(
                    "https://localhost:7217/api/AddToCart/GetCartDetail",
                    { withCredentials: true }
                );
                setCartDetails(response.data);
            } catch (err) {
                setError(err.response?.data || "Error fetching cart details");
            } finally {
                setLoading(false);
            }
        };
        fetchCartDetails();
    }, []);

    if (loading) return <div>Loading...</div>;
    if (error) return <div>{error}</div>;

    const statusData = [
        { name: "Order Placed", value: cartDetails.filter(item => item.status === 1).length },
        { name: "Pending", value: cartDetails.filter(item => item.status !== 1).length },
    ];

    const productData = cartDetails.reduce((acc, item) => {
        const found = acc.find(prod => prod.name === item.productName);
        if (found) {
            found.value += 1;
        } else {
            acc.push({ name: item.productName, value: 1 });
        }
        return acc;
    }, []);

    const COLORS = ["#0088FE", "#FF8042"];

    const exportToExcel = () => {
        const excelData = cartDetails.map((item) => ({
            Username: item.firstName,     
            ProductName: item.productName,
            Price: item.price,
            Status: item.status === 1 ? "Order Placed" : "Pending",
        }));

        const workbook = utils.book_new();

        const cartDetailsSheet = utils.json_to_sheet(excelData);
        utils.book_append_sheet(workbook, cartDetailsSheet, "Cart Details");

        const data = [
            { Sheet: "Order Status", Data: statusData },
            { Sheet: "Product Distribution", Data: productData }
        ];

        data.forEach((item) => {
            const worksheet = utils.json_to_sheet(item.Data);
            utils.book_append_sheet(workbook, worksheet, item.Sheet);
        });

        writeFile(workbook, "CartReports.xlsx");
    };


   const exportGraphToImage = () => {
    const charts = document.querySelectorAll(".chart");
    charts.forEach((chart, index) => {
        toPng(chart).then((dataUrl) => {
            const link = document.createElement("a");
            link.href = dataUrl;
            link.download = `Chart-${index + 1}.png`;
            link.click();
        }).catch((err) => {
            console.error("Failed to export image", err);
        });
    });
};


    return (
        <div className="cart-containers">
            <h2>Cart Reports</h2>
            <div className="export-buttons">
                <button onClick={exportToExcel}>Export to Excel</button>
                <button onClick={exportGraphToImage}>Export Graph as Image</button>
            </div>
            <div className="charts">
                <div className="chart">
                    <h3>Order Status</h3>
                    <ResponsiveContainer width="100%" height={300}>
                        <PieChart>
                            <Pie
                                data={statusData}
                                cx="50%"
                                cy="50%"
                                label
                                outerRadius={100}
                                fill="#8884d8"
                                dataKey="value"
                            >
                                {statusData.map((entry, index) => (
                                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                                ))}
                            </Pie>
                            <Tooltip />
                            <Legend />
                        </PieChart>
                    </ResponsiveContainer>
                </div>

                <div className="chart">
                    <h3>Product Distribution</h3>
                    <ResponsiveContainer width="100%" height={300}>
                        <BarChart data={productData}>
                            <XAxis dataKey="name" />
                            <YAxis  />
                            <Tooltip />
                            <Legend />
                            <Bar dataKey="value" fill="#82ca9d" />
                        </BarChart>
                    </ResponsiveContainer>
                </div>
            </div>
        </div>
    );
};

export default CartDetails;
