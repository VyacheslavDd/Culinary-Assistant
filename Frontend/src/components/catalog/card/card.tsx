import styles from './card.module.scss';
import clock from '../../../assets/svg/clock.svg';
import chef from '../../../assets/svg/chef.svg';
import fire from '../../../assets/svg/fire.svg';
import { ShortRecipe } from 'types';
import {
    transformDifficulty,
    transformRating,
    transformCookingTime,
} from 'utils/transform';
import { useNavigate } from 'react-router-dom';

type CatalogCardProps = {
    recipe: ShortRecipe;
};

export function CatalogCard(props: CatalogCardProps) {
    const navigate = useNavigate();
    const { recipe } = props;
    const {
        id,
        title,
        mainPictureUrl,
        cookingTime,
        cookingDifficulty,
        // category,
        calories,
        // tags,
        // popularity,
        rating,
    } = recipe;

    const handleClick = () => {
        navigate(`/recipe/${id}`);
    };

    return (
        <div className={styles.container} onClick={handleClick}>
            <div
                className={`${styles.rate} 
                ${rating > 8 ? styles.good : rating < 4 ? styles.bad : ''}
                `}
            >
                {transformRating(rating)}
            </div>
            <img src={mainPictureUrl} alt='porridge' className={styles.image} />
            <div className={styles.description}>
                <div className={styles.name}>
                    <p className={styles.title}>{title}</p>
                    <span className={`${styles.icon}`}></span>
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
                        {transformDifficulty(cookingDifficulty)}
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
