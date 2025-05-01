import { HeaderProfile } from 'components/profile/header/header';
import styles from './profile.module.scss';
import { MyCollection } from 'components/profile';
import { Catalog } from 'components/catalog/catalog';
import { useSelector } from 'store/store';
import { selectUserCollections, selectUserRecipes } from 'store/user.slice';
import { NavLink } from 'react-router-dom';

export function ProfilePage() {
    const myRecipes = useSelector(selectUserRecipes);
    const myCollections = useSelector(selectUserCollections);

    return (
        <div className={styles.mainContainer}>
            <HeaderProfile />
            <div className={styles.container}>
                <MyCollection items={myCollections} />
                {myRecipes.length > 0 && (
                    <div className={styles.catalog}>
                        <div className={styles.header}>
                            <h3 className={styles.h3}>Мои рецепты</h3>

                            <NavLink
                                to='/profile/my-recipes'
                                className={styles.link}
                            >
                                Смотреть все
                            </NavLink>
                        </div>
                        <Catalog limit={4} recipes={myRecipes} />
                    </div>
                )}

                {/* <div className={styles.catalog}>
                    <h3 className={styles.h3}>Недавно просмотренные</h3>
                    <Catalog limit={8} />
                </div> */}
            </div>
        </div>
    );
}
