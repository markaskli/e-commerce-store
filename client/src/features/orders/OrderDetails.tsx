import { useEffect, useState } from "react";
import BasketTable from "../baskets/BasketTable";
import LoadingComponent from "../../app/layout/LoadingComponent";
import agent from "../../app/api/agent";
import { BasketItem } from "../../app/models/basket";
import { Order } from "../../app/models/order";
import { Link, useParams } from "react-router-dom";
import { Button, Grid, Typography } from "@mui/material";
import BasketSummary from "../baskets/BasketSummary";

export default function OrderDetails() {
    const [order, setOrder] = useState<Order>();
    const {id} = useParams<{id: string}>();
    const [loading, setLoading] = useState(true);

    const subtotal = order?.orderItems.reduce((sum, item) => sum += (item.price * item.quantity / 100), 0) ?? 0

    

    useEffect(() => {
        agent.Orders.fetch(parseInt(id!))
            .then(order => setOrder(order))
            .catch(error => console.log(error))
            .finally(() => setLoading(false));
    }, [id])

    if (loading) return <LoadingComponent message="Order is being loaded.." />

    return (
        <>
            <Grid container display={"flex"} justifyContent={"space-between"}>
                <Grid item>
                    <Typography variant="h4" gutterBottom>
                        Order# {id} - {order?.orderStatus}
                    </Typography>

                </Grid>
                <Grid item>
                    <Button component={Link} to="/orders" variant="contained">
                        Back to orders
                    </Button>

                </Grid>


            </Grid>



            <BasketTable
                items={order?.orderItems as BasketItem[]}
                isBasket={false}
            />
            <BasketSummary subtotal={subtotal}/>

        </>

    )
}