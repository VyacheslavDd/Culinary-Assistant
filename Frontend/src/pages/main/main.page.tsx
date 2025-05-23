import MainBack from 'components/backgrounds/main-back';
import styles from './main.module.scss';
import { Catalog } from 'components/catalog/catalog';
import { Filter } from './filter';
import { useDispatch, useSelector } from 'store/store';
import {
    fetchRecipes,
    selectMainPageLoading,
    selectPage,
    selectRecipes,
} from 'store/main-page.slice';
import { useEffect } from 'react';
import { Preloader } from 'components/preloader';
import { Helmet } from 'react-helmet-async';

function MainPage() {
    const dispatch = useDispatch();
    const isLoading = useSelector(selectMainPageLoading);
    const page = useSelector(selectPage);
    const recipes = useSelector(selectRecipes);

    useEffect(() => {
        dispatch(fetchRecipes());
    }, [dispatch, page]);

    return (
        <div className={styles.mainContainer}>
            <Helmet>
                <title>КулиНорка</title>
            </Helmet>
            <MainBack />
            <main className={styles.main}>
                <Filter />
                {isLoading ? <Preloader /> : <Catalog recipes={recipes} />}
            </main>
        </div>
    );
}

export default MainPage;
