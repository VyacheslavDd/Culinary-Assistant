import styles from './card-coll-max.module.scss';
import favIcon from '../../../assets/svg/coll_fav_dark.svg';
import { useNavigate } from 'react-router-dom';
import { ShortCollection } from 'types/short-collection.type';
import { transformCreatedAt } from 'utils/transform';
import { COLORS } from 'mocks/colors';

type props = {
    data: ShortCollection;
};

export function CardCollMax(props: props) {
    const { data } = props;
    const navigate = useNavigate();

    const handleClick = () => {
        navigate(`/collection/${data.id}`);
    };

    const MAX_RECIPES_DISPLAY = 6;
    const combinedRecipes = (data.covers || []).map((cover, index) => ({
        url: cover.url,
        title: data.receiptNames?.[index] || 'Без названия',
    }));

    const displayedRecipes = combinedRecipes.slice(0, MAX_RECIPES_DISPLAY);
    const remainingCount = combinedRecipes.length - MAX_RECIPES_DISPLAY;
    const placeholdersCount = MAX_RECIPES_DISPLAY - displayedRecipes.length;

    return (
        <div className={styles.mainContainer} onClick={handleClick}>
            <div className={styles.info}>
                <div className={styles.title}>
                    <div
                        className={styles.name}
                        style={{
                            backgroundColor: COLORS.find(
                                (item) => item.value === data.color
                            )?.color,
                        }}
                    >
                        {data.title}
                    </div>
                    <p>от {data.userLogin}</p>
                </div>

                <div className={styles.rating}>
                    <span
                        className={styles.icon}
                        style={{ backgroundImage: `url(${favIcon})` }}
                    ></span>
                    <span>{data.popularity}</span>
                </div>
            </div>

            <ul className={styles.list}>
                {displayedRecipes.map((recipe, index) => (
                    <li className={styles.item} key={recipe.url || index}>
                        <div className={styles.title}>{recipe.title}</div>
                        <img
                            src={recipe.url}
                            alt='Название рецепта'
                            className={styles.img}
                        />
                    </li>
                ))}

                {Array.from({ length: placeholdersCount }).map((_, index) => (
                    <li className={styles.item} key={`placeholder-${index}`}>
                        <div className={styles.placeholder} />
                    </li>
                ))}

                {remainingCount > 0 && (
                    <li className={styles.item}>
                        <div className={styles.more}>+{remainingCount}</div>
                    </li>
                )}
            </ul>

            <p className={styles.date}>{transformCreatedAt(data.createdAt)}</p>
        </div>
    );
}
