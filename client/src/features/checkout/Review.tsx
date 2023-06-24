import { Grid, Typography } from '@mui/material';
import BasketSummary from '../baskets/BasketSummary';
import BasketTable from '../baskets/BasketTable';
import { useAppSelector } from '../../app/store/configureStore';


export default function Review() {
  const {basket} = useAppSelector(state => state.basket)

  return (
    <>
      <Typography variant="h6" gutterBottom>
        Order summary
      </Typography>
      {basket &&
      <BasketTable items={basket.items} isBasket={false}></BasketTable> }

      <Grid container>
        <Grid item xs={6} />
        <Grid item xs={6}>
          <BasketSummary />

        </Grid>

      </Grid>

    </>
  );
}