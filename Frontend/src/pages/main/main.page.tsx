import MainBack from 'components/backgrounds/main-back';
import styles from './main.module.scss';
import { Catalog } from 'components/catalog/catalog';
import ScrollToTop from 'components/common/scrollToTop';
import { Filter } from './filter';
import { useDispatch, useSelector } from 'store/store';
import {
    fetchRecipes,
    selectFilter,
    selectMainPageLoading,
    selectPage,
    selectRecipes,
} from 'store/main-page.slice';
import { useEffect } from 'react';
import { Preloader } from 'components/preloader';

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
            <MainBack />
            <main className={styles.main}>
                <Filter />
                {isLoading ? <Preloader /> : <Catalog recipes={recipes} />}
            </main>
        </div>
    );
}

export default MainPage;
