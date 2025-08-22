USE mysterybox;

DROP TABLE IF EXISTS `tb_click_count_log`;
CREATE TABLE `tb_click_count_log` (
  `user_id` INT(11) NOT NULL COMMENT '유저의 고유값',
  `log_date` BIGINT(20) NOT NULL COMMENT '로그시간',
  `click_count` INT(11) NOT NULL COMMENT '추가되는 클릭 개수',
  `total_count` BIGINT(20) NOT NULL COMMENT '총 클릭 개수',
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '최근 업데이트 시간',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '최초 생성 시간',
  PRIMARY KEY (`user_id`, `log_date`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='아이템 획득 로그';
