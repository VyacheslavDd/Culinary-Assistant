import { MyCollHeader, OtherCollHeader } from 'components/collection';
import styles from './collection.module.scss';
import { Catalog } from 'components/catalog/catalog';
import ScrollToTop from 'components/common/scrollToTop';
import { useSelector } from 'store/store';
import { selectRecipes } from 'store/main-page.slice';

export function CollectionPage() {
    const recipes = useSelector(selectRecipes);
    return (
        <>
            <ScrollToTop />
            <div className={styles.mainContainer}>
                {/* <MyCollHeader /> */}
                <OtherCollHeader />
                <div className={styles.catalogContainer}>
                    <h2 className={styles.title}> Рецепты</h2>
                    <Catalog recipes={recipes} />
                </div>
            </div>
        </>
    );
}
