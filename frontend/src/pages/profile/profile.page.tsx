import { HeaderProfile } from 'components/profile/header/header';
import styles from './profile.module.scss';
import { Catalog } from 'components/catalog/catalog';
import { useSelector } from 'store/store';
import { selectUserCollections, selectUserRecipes } from 'store/user.slice';
import { NavLink, useNavigate } from 'react-router-dom';
import { ReactComponent as PlusIcon } from '../../assets/svg/button_plus.svg';
import { MyCollection } from 'components/profile/my-collections/my-collection';
import { Helmet } from 'react-helmet-async';

export function ProfilePage() {
    const myRecipes = useSelector(selectUserRecipes);
    const myCollections = useSelector(selectUserCollections);
    const navigate = useNavigate();

    const handleNewRecipe = () => {
        navigate('/recipe/new');
    };

    return (
        <div className={styles.mainContainer}>
            <Helmet>
                <title>Личный кабинет</title>
            </Helmet>
            <HeaderProfile />
            <div className={styles.container}>
                <MyCollection items={myCollections} />
                <div className={styles.catalog}>
                    <div className={styles.header}>
                        <div className={styles.title}>
                            <h3 className={styles.h3}>Мои рецепты</h3>
                            <button
                                className={styles.button}
                                onClick={handleNewRecipe}
                            >
                                <PlusIcon />
                            </button>
                        </div>

                        <NavLink
                            to='/profile/my-recipes'
                            className={styles.link}
                        >
                            Смотреть все
                        </NavLink>
                    </div>
                    {myRecipes.length > 0 && (
                        <Catalog limit={4} recipes={myRecipes} />
                    )}
                </div>

                {/* <div className={styles.catalog}>
                    <h3 className={styles.h3}>Недавно просмотренные</h3>
                    <Catalog limit={8} />
                </div> */}
            </div>
        </div>
    );
}
