import styles from './card.module.scss';
// import photo from '../../../assets/img/Photo.png';
// import icon_favourite from '../../../assets/svg/fav.svg';
import icon_unfavourite from '../../../assets/svg/unfav.svg';
import clock from '../../../assets/svg/clock.svg';
import chef from '../../../assets/svg/chef.svg';
import fire from '../../../assets/svg/fire.svg';
import { ShortRecipe } from 'types';
import { transformDifficulty, transformRating, transformCookingTime } from 'utils/api-transform';

type CatalogCardProps = {
    recipe: ShortRecipe;
};

export function CatalogCard(props: CatalogCardProps) {
    const { recipe } = props;
    const {
        // id,
        title,
        mainImage,
        cookingTime,
        difficulty,
        calories,
        // tags,
        // popularity,
        rating,
    } = recipe;

    return (
        <div className={styles.container}>
            <div
                className={`${styles.rate} 
                ${rating > 8 ? styles.good : rating < 4 ? styles.bad : ''}
                `}
            >
                {transformRating(rating)}
            </div>
            <img src={mainImage} alt='porridge' className={styles.image} />
            <div className={styles.description}>
                <div className={styles.name}>
                    <p className={styles.title}>{title}</p>
                    <img
                        src={icon_unfavourite}
                        alt='unfavourite'
                        className={styles.icon}
                    />
                </div>
                <div className={styles.infoContainer}>
                    <p className={styles.info}>
                        <img
                            src={clock}
                            alt='clock'
                            className={styles.infoIcon}
                        />
                        {transformCookingTime(cookingTime)}
                    </p>
                    <p className={styles.info}>
                        <img
                            src={chef}
                            alt='chef'
                            className={styles.infoIcon}
                        />
                        {transformDifficulty(difficulty)}
                    </p>
                    <p className={styles.info}>
                        <img
                            src={fire}
                            alt='fire'
                            className={styles.infoIcon}
                        />
                        {calories} ккал
                    </p>
                </div>
            </div>
        </div>
    );
}
