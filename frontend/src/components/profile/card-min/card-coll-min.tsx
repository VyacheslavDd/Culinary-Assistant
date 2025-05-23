import styles from './card-coll-min.module.scss';
import favIconLight from '../../../assets/svg/coll_fav_light.svg';
import favIconDark from '../../../assets/svg/coll_fav_dark.svg';
import { useNavigate } from 'react-router-dom';
import { ShortCollection } from 'types/short-collection.type';
import { COLORS } from 'mocks/colors';

type PropsCardCollMin = {
    theme: 'dark' | 'light';
    data: ShortCollection;
};

export function CardCollMin(props: PropsCardCollMin) {
    const { theme, data } = props;
    const iconUrl = theme === 'light' ? favIconLight : favIconDark;

    const navigate = useNavigate();

    const handleClick = () => {
        navigate(`/collection/${data.id}`);
    };

    const MAX_PHOTOS = 6;
    const recipeImages = data.covers?.slice(0, MAX_PHOTOS) ?? [];

    const placeholdersCount = MAX_PHOTOS - recipeImages.length;
    const placeholders = Array.from({ length: placeholdersCount });

    return (
        <div className={styles.mainContainer} onClick={handleClick}>
            <div className={styles.title}  style={{
                                backgroundColor: COLORS.find(
                                    (item) => item.value === data.color
                                )?.color,
                            }}>{data.title}</div>
            <div className={styles.list}>
                {recipeImages.map((recipe, index) => (
                    <div className={styles.item} key={index}>
                        <img
                            src={recipe.url}
                            alt='recipe'
                            className={styles.img}
                        />
                    </div>
                ))}
                {placeholders.map((_, index) => (
                    <div
                        className={styles.item}
                        key={`placeholder-${index}`}
                    ></div>
                ))}
            </div>
            <div className={`${styles.info} ${theme}`}>
                <p className={styles.author}>Автор: {data.userLogin}</p>
                <div className={styles.rating}>
                    <span
                        className={styles.icon}
                        style={{ backgroundImage: `url(${iconUrl})` }}
                    ></span>
                    <span>{data.popularity}</span>
                </div>
            </div>
        </div>
    );
}
