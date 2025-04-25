import styles from './main-info.module.scss';
import star from '../../assets/svg/star.svg';
import clock from '../../assets/svg/clock_pink.svg';
import chef from '../../assets/svg/chef_pink.svg';
import receipt from '../../assets/img/receipt.png';
import { useState } from 'react';
import { Modal } from 'components/common';

export function MainInfo() {
    const [isModalOpen, setIsModalOpen] = useState(false);

    const openModal = () => setIsModalOpen(true);
    const closeModal = () => setIsModalOpen(false);

    return (
        <div className={styles.mainContainer}>
            <button
                onClick={openModal}
                className={styles.imageButton}
                aria-label='Увеличить изображение рецепта'
            >
                <img src={receipt} alt='receipt' className={styles.image} />
            </button>
            <div className={styles.container}>
                <div className={styles.title}>
                    <p className={styles.tag}>Горячее</p>
                    <h2 className={styles.name}>
                        Блинчики с начинкой “Жульен”
                    </h2>
                </div>
                <div className={styles.descriptionContainer}>
                    <div className={styles.infoContainer}>
                        <div className={styles.info}>
                            <p className={styles.title}>Рейтинг</p>
                            <img
                                src={star}
                                alt='star'
                                className={styles.img}
                            ></img>
                            <p className={styles.text}>8.8</p>
                        </div>
                        <div className={styles.info}>
                            <p className={styles.title}>Время приготовления</p>
                            <img
                                src={clock}
                                alt='clock'
                                className={styles.img}
                            ></img>
                            <p className={styles.text}>1.5ч</p>
                        </div>
                        <div className={styles.info}>
                            <p className={styles.title}>Сложность</p>
                            <img
                                src={chef}
                                alt='chef'
                                className={styles.img}
                            ></img>
                            <p className={styles.text}>Средняя</p>
                        </div>
                    </div>
                    <p className={styles.description}>
                        Блинчики с начинкой «Жюльен» — это изысканное сочетание
                        нежных, тонких блинов и ароматной начинки из курицы,
                        грибов и сливочного соуса. Сочная куриная грудка,
                        обжаренные шампиньоны и тягучий сыр создают насыщенный
                        вкус, а сливочный соус придаёт блюду особую мягкость и
                        нежность. Такие блинчики станут отличным вариантом для
                        сытного завтрака, лёгкого ужина или праздничного
                        угощения. Подаются горячими, с зеленью и сметаной.
                    </p>
                    <div className={styles.kbju}>
                        <p className={styles.title}>КБЖУ</p>
                        <p>
                            На 100 г:{' '}
                            <span className={styles.text}>
                                415 / 20 / 24 / 47
                            </span>
                        </p>
                    </div>
                </div>
            </div>
            {isModalOpen && (
                <Modal onClose={closeModal}>
                    <img
                        src={receipt}
                        alt='Увеличенное изображение рецепта'
                        className={styles.fullSizeImage}
                    />
                </Modal>
            )}
        </div>
    );
}
