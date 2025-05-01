import { ButtonWrapper } from 'components/common';
import styles from './buttons.module.scss';

type props = {
    name: string;
};

export function ButtonsReceipt(props: props) {
    const { name } = props;

    const handleAddToCollection = () => {
        console.log('Добавлено в подборку');
    };

    const handleFavourite = () => {
        console.log('Добавлено в подборку');
    };

    const handleShare = () => {
        const shareData = {
            title: name,
            text: 'Посмотри этот потрясающий рецепт!',
            url: window.location.href,
        };

        if (navigator.share) {
            navigator
                .share(shareData)
                .catch((err) =>
                    console.error('Ошибка при попытке поделиться:', err)
                );
        } else {
            navigator.clipboard
                .writeText(
                    `${shareData.title}
                ${shareData.url}`
                )
                .then(() => {
                    alert('Ссылка скопирована в буфер обмена!');
                });
        }
    };

    return (
        <div className={styles.mainContainer}>
            <ButtonWrapper onAuthenticatedAction={handleFavourite}>
                <button className={`button ${styles.buttonShare}`}>
                    <span className={`${styles.icon} ${styles.like}`}></span>
                    Нравится
                </button>
            </ButtonWrapper>

            <button
                className={`button ${styles.buttonShare}`}
                onClick={handleShare}
            >
                <span className={`${styles.icon} ${styles.share}`}></span>
                Поделиться
            </button>

            <ButtonWrapper onAuthenticatedAction={handleAddToCollection}>
                <button className={styles.buttonAdd}>
                    <span className={`${styles.icon} ${styles.add}`}></span>
                    Добавить в подборку
                </button>
            </ButtonWrapper>
        </div>
    );
}
